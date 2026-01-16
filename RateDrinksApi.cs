using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using RateDrinksApi;
using RateDrinksApi.Models;
using RateDrinksApi.Repositories;
using RateDrinksApi.Services;

// System.Text.Json source generation context for serialization
var drinksJsonOptions = new System.Text.Json.JsonSerializerOptions
{
    TypeInfoResolver = DrinksJsonContext.Default
};
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddOpenApi();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, DrinksJsonContext.Default);
});

// Register repositories and services for drinks
builder.Services.AddSingleton<IAlcoholicDrinkRepository<Beer>, InMemoryAlcoholicDrinkRepository<Beer>>();
builder.Services.AddSingleton<IAlcoholicDrinkRepository<Wine>, InMemoryAlcoholicDrinkRepository<Wine>>();
builder.Services.AddSingleton<IAlcoholicDrinkRepository<Vodka>, InMemoryAlcoholicDrinkRepository<Vodka>>();

builder.Services.AddTransient<AlcoholicDrinkService<Beer>>();
builder.Services.AddTransient<AlcoholicDrinkService<Wine>>();
builder.Services.AddTransient<AlcoholicDrinkService<Vodka>>();
/*
builder.Services.AddSingleton<AlcoholicDrinkService<Beer>>();
builder.Services.AddSingleton<AlcoholicDrinkService<Wine>>();
builder.Services.AddSingleton<AlcoholicDrinkService<Vodka>>();
*/
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


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

// Use rate limiting middleware
app.UseRateLimiter();


// ...existing code...


// Endpoints
app.MapGet("/text", () => {
    System.Console.WriteLine("Endpoint /text called");
    return "Hello From Net10!";
}).RequireRateLimiting("default");
app.MapGet("/text/{userName}", (string userName) => $"Hello {userName} From Net10!").RequireRateLimiting("default");

// CRUD endpoints for Beer
app.MapGet("/beers", (AlcoholicDrinkService<Beer> service) =>
{
    var beers = service.GetAll() ?? Enumerable.Empty<Beer>();
    return Results.Json(beers, drinksJsonOptions);
});
app.MapGet("/beers/{id:int}", (int id, AlcoholicDrinkService<Beer> service) =>
{
    var beer = service.GetById(id);

    return beer is not null ? Results.Json(beer, drinksJsonOptions) : Results.NotFound();
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
});
app.MapPost("/beers", (List<Beer> beers, AlcoholicDrinkService<Beer> service) =>
{
    var added = new List<Beer>();
    foreach (var beer in beers)
    {
        service.Add(beer);
        added.Add(beer);
    }
    return Results.Created($"/beers", added);
});
app.MapPut("/beers/{id:int}", (int id, Beer beer, AlcoholicDrinkService<Beer> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    beer.Id = id;
    service.Update(beer);
    return Results.NoContent();
});
app.MapDelete("/beers/{id:int}", (int id, AlcoholicDrinkService<Beer> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    service.Delete(id);
    return Results.NoContent();
});

// CRUD endpoints for Wine
app.MapGet("/wines", (AlcoholicDrinkService<Wine> service) =>
{
    var wines = service.GetAll() ?? Enumerable.Empty<Wine>();
    return Results.Json(wines, drinksJsonOptions);
});
app.MapGet("/wines/{id:int}", (int id, AlcoholicDrinkService<Wine> service) =>
{
    var wine = service.GetById(id);
    return wine is not null ? Results.Json(wine, drinksJsonOptions) : Results.NotFound();
});
app.MapPost("/wines", (List<Wine> wines, AlcoholicDrinkService<Wine> service) =>
{
    var added = new List<Wine>();
    foreach (var wine in wines)
    {
        service.Add(wine);
        added.Add(wine);
    }
    return Results.Created($"/wines", added);
});
app.MapPut("/wines/{id:int}", (int id, Wine wine, AlcoholicDrinkService<Wine> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    wine.Id = id;
    service.Update(wine);
    return Results.NoContent();
});
app.MapDelete("/wines/{id:int}", (int id, AlcoholicDrinkService<Wine> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    service.Delete(id);
    return Results.NoContent();
});

// CRUD endpoints for Vodka
app.MapGet("/vodkas", (AlcoholicDrinkService<Vodka> service) =>
{
    var vodkas = service.GetAll() ?? Enumerable.Empty<Vodka>();
    return Results.Json(vodkas, drinksJsonOptions);
});
app.MapGet("/vodkas/{id:int}", (int id, AlcoholicDrinkService<Vodka> service) =>
{
    var vodka = service.GetById(id);
    return vodka is not null ? Results.Json(vodka, drinksJsonOptions) : Results.NotFound();
});
app.MapPost("/vodkas", (List<Vodka> vodkas, AlcoholicDrinkService<Vodka> service) =>
{
    var added = new List<Vodka>();
    foreach (var vodka in vodkas)
    {
        service.Add(vodka);
        added.Add(vodka);
    }
    return Results.Created($"/vodkas", added);
});
app.MapPut("/vodkas/{id:int}", (int id, Vodka vodka, AlcoholicDrinkService<Vodka> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    vodka.Id = id;
    service.Update(vodka);
    return Results.NoContent();
});
app.MapDelete("/vodkas/{id:int}", (int id, AlcoholicDrinkService<Vodka> service) =>
{
    if (service.GetById(id) is null) return Results.NotFound();
    service.Delete(id);
    return Results.NoContent();
});

// Terminal middleware that runs the endpoint pipeline
app.Run();
