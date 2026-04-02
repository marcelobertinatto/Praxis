using Microsoft.AspNetCore.RateLimiting;
using Praxis.Features.Stories;
using Praxis.Infra;
using Praxis.Infra.Cache;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Redis
var redisConnection = builder.Configuration.GetValue<string>("Redis:ConnectionString");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});

// Core services
builder.Services.AddSingleton<InFlightRequestCacheService>();
builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddScoped<IStoryService, StoryService>();

builder.Services.AddHttpClient<HttpHackerNewsClient>(client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
})
.AddStandardResilienceHandler();

// Rate limiting
builder.Services.AddRateLimiter(opt =>
{
    opt.AddFixedWindowLimiter("fixed", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromSeconds(10);
    });
});


var app = builder.Build();

app.UseRateLimiter();
app.MapOpenApi();

// Endpoint
app.MapStoryEndpoints();

app.UseHttpsRedirection();

// Add Scalar UI
app.MapScalarApiReference();

app.Run();
