using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using BurgerHub.Api.Seeder.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BurgerHub.Api.Seeder;

public interface ISeedingService
{
    Task SeedAsync();
}

public class SeedingService : ISeedingService
{
    private readonly IFakerFactory _fakerFactory;
    
    private readonly IOptions<SeedingOptions> _seedingOptions;

    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Review> _reviewCollection;
    private readonly IMongoCollection<Restaurant> _restaurantCollection;
    private readonly IMongoCollection<Photo> _photoCollection;

    public SeedingService(
        IFakerFactory fakerFactory,
        IOptions<SeedingOptions> seedingOptions,
        IMongoCollection<User> userCollection,
        IMongoCollection<Review> reviewCollection,
        IMongoCollection<Restaurant> restaurantCollection,
        IMongoCollection<Photo> photoCollection)
    {
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

        var users = userFaker.Generate(_seedingOptions.Value.AmountOfUsers);
        await _userCollection.InsertManyAsync(users);

        await Task.WhenAll(
            Task.WhenAll(users.Select(SeedPhotosForUserAsync)),
            Task.WhenAll(users.Select(SeedReviewsForUserAsync)));
    }

    private async Task SeedPhotosForUserAsync(User user)
    {
        var photoFaker = _fakerFactory.CreatePhotoFaker();
        
        var photos = photoFaker.Generate(_seedingOptions.Value.AmountOfPhotosPerUser);
        photos.ForEach(p => p.AuthorUserId = user.Id);

        await _photoCollection.InsertManyAsync(photos);
    }

    private async Task SeedReviewsForUserAsync(User user)
    {
        var reviewFaker = _fakerFactory.CreateReviewFaker();
        
        var reviews = reviewFaker.Generate(_seedingOptions.Value.AmountOfReviewsPerUser);
        reviews.ForEach(p => p.AuthorUserId = user.Id);

        await _reviewCollection.InsertManyAsync(reviews);
    }

    private async Task SeedRestaurantsAsync()
    {
        var restaurantFaker = _fakerFactory.CreateRestaurantFaker();

        for (var i = 0; i < _seedingOptions.Value.AmountOfRestaurants; i++)
        {
            var restaurant = restaurantFaker.Generate();
            await _restaurantCollection.InsertOneAsync(restaurant);
        }
    }
}