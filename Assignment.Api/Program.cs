using Assignment.App.Data;
using Assignment.LoaderConsole;
using LazyCache;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// asp.net
{
    builder.Services.AddLogging(q => q.AddConsole());
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen(c =>
    {
        c.ResolveConflictingActions(apiDescription => apiDescription.Last());

        const string API_KEY_SCHEME = "ApiKey";

        c.AddSecurityDefinition(API_KEY_SCHEME, new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = $"Input format: {API_KEY_SCHEME} the-key"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = API_KEY_SCHEME,
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// infra
{
    builder.Services.AddSingleton<IAppCache>(q => new CachingService());
}

// deps
{
    builder.Services.AddScoped(q => new StatisticsContext());

    builder.Services.AddDbContext<Context>();
    builder.Services.AddScoped<IRepository>(q => new Repository(
        q.GetRequiredService<Context>(),
        q.GetRequiredService<StatisticsContext>()
    ));

    var cacheName = $"data-cache-for-{nameof(RickAndMortyService)}";

    builder.Services.AddKeyedSingleton<IAppCache>(cacheName, (q, _) => new LazyCache.CachingService());
    builder.Services.AddScoped<IRickAndMortyService>(q => new RickAndMortyService(
        q.GetRequiredService<IRepository>(),
        q.GetRequiredKeyedService<IAppCache>(cacheName),
        q.GetRequiredService<StatisticsContext>()
    ));
}

// app
{
    var app = builder.Build();

    var logger = app.Services.GetRequiredService<ILogger<ILogger>>();


    app.UseSwagger();

    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Use((context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["from-database"] = context.RequestServices.GetRequiredService<StatisticsContext>().IsFromDatabase ? "1" : "0";
            return Task.CompletedTask;
        });

        return next(context);
    });

    try
    {
        await app.StartAsync();

        logger.LogInformation("started");

        await app.WaitForShutdownAsync();

        logger.LogInformation("stopped");
    }
    catch (Exception e)
    {
        logger.LogInformation(e, "startup failed");
        throw;
    }
}

string REQUIRE(string key)
{
    var value = config.GetValue<string>(key);

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new Exception($"Missing configuration value for key '{key}'");
    }

    return value;
}