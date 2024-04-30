using Dummy.Infrastructure.Extensions;
using Dummy.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

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
                .FluentValidationConfiguration()
                .MediatRConfiguration();



var app = builder.Build();

// Middlewares
app.UseExceptionHandler(opt => { }); // Global Exception Handler

var supportedCultures = new[] { "vi-VN", "en-US" };
IList<CultureInfo> cultures = new List<CultureInfo>();
foreach (string lang in supportedCultures)
{
    cultures.Add(new CultureInfo(lang));
}

app.UseRequestLocalization(opt =>
{
    opt.SetDefaultCulture("vi-VN");
    opt.SupportedCultures = cultures;
    opt.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
});

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
