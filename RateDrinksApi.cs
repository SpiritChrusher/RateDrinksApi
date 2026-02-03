using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using RateDrinksApi;
using RateDrinksApi.Models;
using RateDrinksApi.Repositories;
using RateDrinksApi.Services;
using RateDrinksApi.Extensions;
using RateDrinksApi.Models.Dto;

// System.Text.Json source generation context for serialization
var drinksJsonOptions = new System.Text.Json.JsonSerializerOptions
{
    TypeInfoResolver = DrinksJsonContext.Default
};

var builder = WebApplication.CreateBuilder(args);
var appName = builder.Environment.ApplicationName;

// Add Azure App Configuration
var appConfigUri = builder.Configuration["AppConfiguration"]!;
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfigUri), new Azure.Identity.DefaultAzureCredential());
});

// JWT authentication with JWKS from blob storage using JWKSOptions
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwksUrl = builder.Configuration[$"{appName}:JWKS:Uri"];
        // Reuse a static HttpClient for performance
        var staticHttpClient = JwksHttpClient.Instance;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "rate-drinks",
            ValidAudience = "rate-drinks-users",
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var jwksJson = staticHttpClient.GetStringAsync(jwksUrl).Result;
                var jwks = new JsonWebKeySet(jwksJson);
                return jwks.Keys;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
});

// Add CORS policy to allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});
// Use app-scoped config for all options
builder.Services.AddAppConfiguration(builder.Configuration, appName);
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

builder.Services.AddScoped<IDrinksService, DrinksService>();
// Register Cosmos DB repository for drinks
builder.Services.AddScoped<IDrinksRepository, DrinksRepository>();
builder.Services.AddScoped<IDrinksService, DrinksService>();
builder.Services.AddScoped<IRatingRepository, PgRatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();

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
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before any endpoints
app.UseCors("AllowFrontend");

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
}).RequireAuthorization();

app.MapGet("/ratings/{drinkId}", async (string drinkId, IRatingService ratingService) =>
{
    var ratings = (await ratingService.GetRatingsForDrinkAsync(drinkId)).ToList();
    return Results.Json(ratings, DrinksJsonContext.Default.ListRating);
}).CacheOutput(p => p.Expire(TimeSpan.FromSeconds(30)).SetVaryByRouteValue("drinkId")).RequireAuthorization();

// Endpoint for average rating
app.MapGet("/ratings/{drinkId}/average", async (string drinkId, IRatingService ratingService) =>
{
    var avg = await RatingExtensions.GetAverageRatingAsync(ratingService, drinkId);
    return avg.HasValue
        ? Results.Json(new AverageRatingResponse { DrinkId = drinkId, Average = avg.Value }, DrinksJsonContext.Default.AverageRatingResponse)
        : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/ratings/{drinkId:int}/average", async (string drinkId, IRatingService ratingService) =>
{
    var avg = await RatingExtensions.GetAverageRatingAsync(ratingService, drinkId);
    return avg.HasValue
        ? Results.Json(new AverageRatingResponse { DrinkId = drinkId, Average = avg.Value }, DrinksJsonContext.Default.AverageRatingResponse)
        : Results.NotFound();
}).RequireAuthorization();


// Unified CRUD endpoints for all drink types

app.MapGet("/drinks", async (AlcoholType type, IDrinksService drinkService) =>
{
    var drinks = await drinkService.GetAllDrinksAsync(type);
    return Results.Json(drinks, DrinksJsonContext.Default.ListAlcoholicDrink);
}).RequireAuthorization();


app.MapGet("/drinks/{id}", async (string id, IDrinksService drinkService) =>
{
    var drink = await drinkService.GetDrinkByIdAsync(id);
    return drink is not null
        ? Results.Json(drink, DrinksJsonContext.Default.AlcoholicDrink)
        : Results.NotFound();
}).RequireAuthorization();



app.MapPost("/drinks", async (HttpContext http, List<AddDrinkDto> drinkDtos, IDrinksService drinkService) =>
{
    if (!http.User.IsAdmin())
        return Results.Forbid();

    var (drinks, errors) = drinkDtos.ToAlcoholicDrinks();
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    var (added, addErrors) = await drinkService.AddDrinksAsync(drinks);
    if (addErrors.Count > 0)
        return Results.BadRequest(new { errors = addErrors });
    
    return Results.Created($"/drinks", added);
}).RequireAuthorization();


app.MapPut("/drinks/{id}", async (HttpContext http, string id, AlcoholicDrink drink, IDrinksService drinkService) =>
{
    if (!http.User.IsAdmin())
        return Results.Forbid();

    var (success, notFound, error) = await drinkService.UpdateDrinkAsync(id, drink);
    if (notFound) 
        return Results.NotFound();

    if (!success) 
        return Results.BadRequest(new { error });

    return Results.NoContent();
}).RequireAuthorization();


app.MapDelete("/drinks/{id}", async (string id, IDrinksService drinkService) =>
{
    var (success, notFound) = await drinkService.DeleteDrinkAsync(id);
    return notFound ? Results.NotFound() : Results.NoContent();
}).RequireAuthorization();

app.Run();

// Static HttpClient for JWKS fetching
public static class JwksHttpClient
{
    private static readonly HttpClient _instance = new();
    public static HttpClient Instance => _instance;
}
