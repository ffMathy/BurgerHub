using BurgerHub.Api.Infrastructure;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

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
