using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BurgerHub.Api.Infrastructure;

public class ApiIocRegistry
{
    private readonly IServiceCollection _serviceCollection;

    public ApiIocRegistry(
        IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void Register()
    {
        RegisterOptions();
        RegisterMongo();
    }

    private void RegisterOptions()
    {
        _serviceCollection.AddOptions<MongoOptions>("Mongo");
        _serviceCollection.AddOptions<EncryptionOptions>("Encryption");
    }

    private void RegisterMongo()
    {
        _serviceCollection.AddScoped<IMongoDatabase>(provider =>
        {
            var options = provider.GetService<IOptions<MongoOptions>>();
            var connectionString =
                options?.Value.ConnectionString ??
                throw new InvalidOperationException("Mongo connection string not found.");
            
            var client = new MongoClient(connectionString);
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