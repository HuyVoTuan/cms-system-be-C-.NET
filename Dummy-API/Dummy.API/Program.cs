using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Dummy API with JWT Token Authentication",
        Description = ".NET 8 Web API"
    });
    // Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}

        }
    });
});


// DI
builder.Services.AuthConfiguration(builder.Configuration)
                .DatabaseConfiguration(builder.Configuration)
                .AddExceptionHandler<RestfulAPIExceptionHandler>()
                .ServicesConfiguration()
                .RedisConfiguration(builder.Configuration)
                .LocalizationConfiguration(builder.Configuration)
                .FluentValidationConfiguration()
                .EmailNotificationConfiguration(builder.Configuration)
                .QuartzConfiguration(builder.Configuration)
                .MediatRConfiguration();

var app = builder.Build();

// Middlewares
app.UseExceptionHandler(opt => { }); // Global Exception Handler
app.UseMiddleware<LocalizationMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication - Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
