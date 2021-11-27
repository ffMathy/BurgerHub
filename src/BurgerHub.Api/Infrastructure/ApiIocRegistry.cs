using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using Microsoft.Extensions.Options;
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
        RegisterOptions();
        RegisterMongo();
        RegisterEncryption();
    }

    private void RegisterEncryption()
    {
        _serviceCollection.AddScoped<IEncryptionHelper, EncryptionHelper>();
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