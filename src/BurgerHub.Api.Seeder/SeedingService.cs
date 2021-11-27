using Bogus;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using BurgerHub.Api.Seeder.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BurgerHub.Api.Seeder;

public interface ISeedingService
{
    Task SeedAsync();
}

public class SeedingService : ISeedingService
{
    private readonly IAesEncryptionHelper _aesEncryptionHelper;
    private readonly IFakerFactory _fakerFactory;
    
    private readonly IOptions<SeedingOptions> _seedingOptions;

    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Review> _reviewCollection;
    private readonly IMongoCollection<Restaurant> _restaurantCollection;
    private readonly IMongoCollection<Photo> _photoCollection;

    public SeedingService(
        IAesEncryptionHelper aesEncryptionHelper,
        IFakerFactory fakerFactory,
        IOptions<SeedingOptions> seedingOptions,
        IMongoCollection<User> userCollection,
        IMongoCollection<Review> reviewCollection,
        IMongoCollection<Restaurant> restaurantCollection,
        IMongoCollection<Photo> photoCollection)
    {
        _aesEncryptionHelper = aesEncryptionHelper;
        _fakerFactory = fakerFactory;
        _seedingOptions = seedingOptions;
        _userCollection = userCollection;
        _reviewCollection = reviewCollection;
        _restaurantCollection = restaurantCollection;
        _photoCollection = photoCollection;
    }
    
    public async Task SeedAsync()
    {
        await Task.WhenAll(
            SeedUsersAsync(),
            SeedRestaurantsAsync());
    }

    private async Task SeedUsersAsync()
    {
        var userFaker = _fakerFactory.CreateUserFaker();

        for (var i = 0; i < _seedingOptions.Value.AmountOfUsers; i++)
        {
            var user = userFaker.Generate();
            await _userCollection.InsertOneAsync(user);
        }
    }

    private async Task SeedRestaurantsAsync()
    {
        var restaurantFaker = _fakerFactory.CreateRestaurantFaker();

        for (var i = 0; i < _seedingOptions.Value.AmountOfUsers; i++)
        {
            var restaurant = restaurantFaker.Generate();
            await _restaurantCollection.InsertOneAsync(restaurant);
        }
    }
}