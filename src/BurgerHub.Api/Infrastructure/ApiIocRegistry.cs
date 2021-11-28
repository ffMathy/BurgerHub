using System.Text;
using System.Text.Json.Serialization;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace BurgerHub.Api.Infrastructure;

public class ApiIocRegistry
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;

    public ApiIocRegistry(
        IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        _serviceCollection = serviceCollection;
        _configuration = configuration;
    }

    public void Register()
    {
        RegisterAspNet();
        RegisterOptions();
        RegisterMongo();
        RegisterSecurity();
    }

    private void RegisterAspNet()
    {
        _serviceCollection
            .AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        _serviceCollection.AddSwaggerGen();

        _serviceCollection
            .AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = _configuration.GetOptions<JwtOptions>();
                options.Audience = jwtOptions.Audience;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateAudience = true,
                    ValidateActor = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateTokenReplay = false,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.PrivateKey))
                };
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async (context) =>
                    {
                        context.HandleResponse();
                            
                        if (context.AuthenticateFailure != null)
                        {
                            context.Response.StatusCode = 401;
                            await context.HttpContext.Response.WriteAsync("{}");
                        }
                    }
                };
            });
    }

    private void RegisterSecurity()
    {
        _serviceCollection.AddScoped<IEncryptionHelper, EncryptionHelper>();
        _serviceCollection.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
        _serviceCollection.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
    }

    private void RegisterOptions()
    {
        void Configure<TOptions>(string name) where TOptions : class
        {
            _serviceCollection.Configure<TOptions>(
                _configuration.GetSection(name));
        }
        
        _serviceCollection.AddOptions();
        
        Configure<MongoOptions>("Mongo");
        Configure<EncryptionOptions>("Encryption");
        Configure<JwtOptions>("Jwt");
    }

    private void RegisterMongo()
    {
        _serviceCollection.AddScoped<IMongoDatabase>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MongoOptions>>();
            
            var client = new MongoClient(options.Value.ConnectionString);
            return client.GetDatabase(options.Value.DatabaseName);
        });
        
        RegisterMongoCollection<User>();
        RegisterMongoCollection<Review>();
        RegisterMongoCollection<Restaurant>();
        RegisterMongoCollection<Photo>();
    }

    private void RegisterMongoCollection<TCollection>()
    {
        _serviceCollection.AddTransient<IMongoCollection<TCollection>>(
            provider =>
            {
                var database = provider.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<TCollection>(typeof(TCollection).Name);
            });
    }
}