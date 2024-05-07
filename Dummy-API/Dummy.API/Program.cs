using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DI
builder.Services.AuthConfiguration(builder.Configuration)
                .DatabaseConfiguration(builder.Configuration)
                .AddExceptionHandler<RestfulAPIExceptionHandler>()
                .ServicesConfiguration()
                .RedisConfiguration(builder.Configuration)
                .LocalizationConfiguration(builder.Configuration)
                .FluentValidationConfiguration()
                .EmailNotificationConfiguration(builder.Configuration)
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
