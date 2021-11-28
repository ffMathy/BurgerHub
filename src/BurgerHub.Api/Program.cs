using BurgerHub.Api.Infrastructure;
using Destructurama;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Verbose()
    .Destructure.UsingAttributes());

var registry = new ApiIocRegistry(
    builder.Services,
    builder.Configuration);
registry.Register();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
