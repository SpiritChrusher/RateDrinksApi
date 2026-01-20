using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using RateDrinksApi;
using RateDrinksApi.Models;
using RateDrinksApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RateDrinksApi.Repositories;
using RateDrinksApi.Services;
using RateDrinksApi.Extensions;
using RateDrinksApi.Options;

// System.Text.Json source generation context for serialization
var drinksJsonOptions = new System.Text.Json.JsonSerializerOptions
{
    TypeInfoResolver = DrinksJsonContext.Default
};
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RateDrinksApi.Options.DatabaseOptions>(
    builder.Configuration.GetSection("DatabaseSettings"));
// Add services to the container.x
builder.Services.AddOutputCache(options =>
{
    // Default cache duration for all endpoints (in seconds)
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddOpenApi();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, DrinksJsonContext.Default)
);

builder.Services.AddScoped<IAlcoholicDrinkService, AlcoholicDrinkService>();
builder.Services.AddDbContext<DrinksDbContext>((sp, options) =>
{
    var dbOpts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RateDrinksApi.Options.DatabaseOptions>>().Value;
    var configValue = dbOpts.ConnectionStrings.AZURE_SQL_CONNECTIONSTRING;
    var connectionString = configValue == "USE_ENVIRONMENT_VARIABLE"
        ? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING") ?? string.Empty
        : configValue;
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IRatingRepository, PgRatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IAlcoholicDrinkService, AlcoholicDrinkService>();

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 15,
                Window = TimeSpan.FromSeconds(22),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }
        )
    );
    options.AddFixedWindowLimiter("default", opt =>
    {
        opt.PermitLimit = 60;
        opt.Window = TimeSpan.FromSeconds(66);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddScoped<IRatingService, RatingService>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();

// Exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    Console.WriteLine("An exception occurred.");
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsync($"{{\"error\":\"{error.Error.Message}\"}}");
        }
    });
});

app.UseRateLimiter();

app.MapGet("/health", () => {
    return "Hello From Net10!";
}).RequireRateLimiting("default");


// Rating endpoints
app.MapPost("/ratings", async (Rating rating, IRatingService ratingService) =>
{
    await ratingService.AddRatingAsync(rating);
    return Results.Created($"/ratings", rating);
});

app.MapGet("/ratings/{drinkId}", async (int drinkId, IRatingService ratingService) =>
{
    var ratings = (await ratingService.GetRatingsForDrinkAsync(drinkId)).ToList();
    return Results.Json(ratings, DrinksJsonContext.Default.ListRating);
}).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(30)).SetVaryByRouteValue("drinkId"));

app.MapGet("/ratings/{drinkId:int}", async (int drinkId, IRatingService ratingService) =>
{
    var ratings = (await ratingService.GetRatingsForDrinkAsync(drinkId)).ToList();
    return Results.Json(ratings, DrinksJsonContext.Default.ListRating);
}).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(30)).SetVaryByRouteValue("drinkId"));

// Endpoint for average rating
app.MapGet("/ratings/{drinkId}/average", async (int drinkId, IRatingService ratingService) =>
{
    var avg = await RatingExtensions.GetAverageRatingAsync(ratingService, drinkId);
    return avg.HasValue
        ? Results.Json(new AverageRatingResponse { DrinkId = drinkId, Average = avg.Value }, DrinksJsonContext.Default.AverageRatingResponse)
        : Results.NotFound();
});

app.MapGet("/ratings/{drinkId:int}/average", async (int drinkId, IRatingService ratingService) =>
{
    var avg = await RatingExtensions.GetAverageRatingAsync(ratingService, drinkId);
    return avg.HasValue
        ? Results.Json(new AverageRatingResponse { DrinkId = drinkId, Average = avg.Value }, DrinksJsonContext.Default.AverageRatingResponse)
        : Results.NotFound();
});


// Unified CRUD endpoints for all drink types
app.MapGet("/drinks", (IAlcoholicDrinkService drinkService) =>
{
    var drinks = drinkService.GetAllDrinks().ToList();
    return Results.Json(drinks, DrinksJsonContext.Default.ListAlcoholicDrink);
});

app.MapGet("/drinks/{id:int}", (int id, IAlcoholicDrinkService drinkService) =>
{
    var drink = drinkService.GetDrinkById(id);
    return drink is not null
        ? Results.Json(drink, DrinksJsonContext.Default.AlcoholicDrink)
        : Results.NotFound();
});

app.MapPost("/drinks", (List<DrinkRecord> drinks, IAlcoholicDrinkService drinkService) =>
{
    // Convert DrinkRecord to AlcoholicDrink for validation and service logic
    var alcoholicDrinks = drinks
        .Select(AlcoholicDrinkService.FromRecord)
        .Where(d => d is not null)
        .Cast<AlcoholicDrink>()
        .ToList();
    var (added, errors) = drinkService.AddDrinks(alcoholicDrinks);
    if (errors.Count > 0)
    {
        return Results.BadRequest(new { errors });
    }
    return Results.Created($"/drinks", added);
});

app.MapPut("/drinks/{id:int}", (int id, AlcoholicDrink drink, IAlcoholicDrinkService drinkService) =>
{
    var (success, notFound, error) = drinkService.UpdateDrink(id, drink);
    if (notFound) return Results.NotFound();
    if (!success) return Results.BadRequest(new { error });
    return Results.NoContent();
});

app.MapDelete("/drinks/{id:int}", (int id, IAlcoholicDrinkService drinkService) =>
{
    var (success, notFound) = drinkService.DeleteDrink(id);
    return notFound ? Results.NotFound() : Results.NoContent();
});

app.Run();
