using Bogus;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using BurgerHub.Api.Seeder.Infrastructure;
using Microsoft.AspNetCore.Identity;
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
    private readonly IEncryptionHelper _encryptionHelper;
    
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IOptions<SeedingOptions> _seedingOptions;

    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Review> _reviewCollection;
    private readonly IMongoCollection<Restaurant> _restaurantCollection;
    private readonly IMongoCollection<Photo> _photoCollection;

    //TODO: SRP violation? https://github.com/ffMathy/BurgerHub/issues/5
    public SeedingService(
        IFakerFactory fakerFactory,
        IPasswordHasher<User> passwordHasher,
        IEncryptionHelper encryptionHelper,
        IOptions<SeedingOptions> seedingOptions,
        IMongoCollection<User> userCollection,
        IMongoCollection<Review> reviewCollection,
        IMongoCollection<Restaurant> restaurantCollection,
        IMongoCollection<Photo> photoCollection)
    {
        _fakerFactory = fakerFactory;
        _passwordHasher = passwordHasher;
        _encryptionHelper = encryptionHelper;
        _seedingOptions = seedingOptions;
        _userCollection = userCollection;
        _reviewCollection = reviewCollection;
        _restaurantCollection = restaurantCollection;
        _photoCollection = photoCollection;
    }
    
    public async Task SeedAsync()
    {
        var restaurants = await SeedRestaurantsAsync();
        await SeedUsersAsync(restaurants);
    }

    private async Task SeedUsersAsync(
        IReadOnlyList<Restaurant> allRestaurants)
    {
        await _userCollection.DeleteManyAsync(FilterDefinition<User>.Empty);

        var userFaker = _fakerFactory.CreateUserFaker();

        var users = userFaker.Generate(_seedingOptions.Value.AmountOfUsers);

        //for convenience (to always be able to login with a known user), we hard-code the e-mail address of the first user.
        //all other users are generated randomly.
        var firstUser = users.First();
        await SetUserCredentialsAsync(
            firstUser, 
            email: "mathias@example.com",
            password: "123456");

        await SetRandomUserCredentialsForEveryUserExceptAsync(
            users, 
            exceptUser: firstUser);
        
        await _userCollection.InsertManyAsync(users);

        await Task.WhenAll(
            Task.WhenAll(users.Select(SeedPhotosForUserAsync)),
            Task.WhenAll(users.Select(user => SeedReviewsForUserAsync(
                user,
                allRestaurants))));
    }

    private async Task SetRandomUserCredentialsForEveryUserExceptAsync(
        IEnumerable<User> users, 
        User exceptUser)
    {
        var faker = new Faker();
        await Task.WhenAll(users
            .Except(new[] { exceptUser })
            .Select(user => SetUserCredentialsAsync(
                user,
                faker.Internet.Email(),
                faker.Internet.Password())));
    }

    private async Task SetUserCredentialsAsync(
        User user,
        string email,
        string password)
    {
        user.HashedPassword = _passwordHasher.HashPassword(user, password);
        user.EncryptedEmail = await _encryptionHelper.EncryptAsync(
            email,
            withoutSalt: true);
    }

    private async Task SeedPhotosForUserAsync(User user)
    {
        await _photoCollection.DeleteManyAsync(FilterDefinition<Photo>.Empty);
        
        var photoFaker = _fakerFactory.CreatePhotoFaker();
        
        var photos = photoFaker.Generate(_seedingOptions.Value.AmountOfPhotosPerUser);
        photos.ForEach(p => p.AuthorUserId = user.Id);

        await _photoCollection.InsertManyAsync(photos);
    }

    private async Task SeedReviewsForUserAsync(
        User user,
        IReadOnlyList<Restaurant> allRestaurants)
    {
        await _reviewCollection.DeleteManyAsync(FilterDefinition<Review>.Empty);

        await _reviewCollection.Indexes.DropAllAsync();
        await _reviewCollection.Indexes.CreateOneAsync(new CreateIndexModel<Review>(
            Builders<Review>.IndexKeys
                .Ascending(x => x.RestaurantId)
                .Ascending(x => x.AuthorUserId),
            new CreateIndexOptions()
            {
                Unique = true
            }));

        var faker = new Faker();
        var reviewFaker = _fakerFactory.CreateReviewFaker();
        
        var reviews = reviewFaker.Generate(_seedingOptions.Value.AmountOfReviewsPerUser);
        reviews.ForEach(r =>
        {
            r.AuthorUserId = user.Id;
            r.RestaurantId = faker.PickRandom(allRestaurants.ToArray()).Id;
        });

        await _reviewCollection.InsertManyAsync(reviews);
    }

    private async Task<IReadOnlyList<Restaurant>> SeedRestaurantsAsync()
    {
        await _restaurantCollection.DeleteManyAsync(FilterDefinition<Restaurant>.Empty);

        await _restaurantCollection.Indexes.DropAllAsync();
        await _restaurantCollection.Indexes.CreateOneAsync(new CreateIndexModel<Restaurant>(
            Builders<Restaurant>.IndexKeys.Geo2DSphere(x => x.Location)));
        
        var restaurantFaker = _fakerFactory.CreateRestaurantFaker();
        
        var restaurants = restaurantFaker.Generate(_seedingOptions.Value.AmountOfRestaurants);
        await _restaurantCollection.InsertManyAsync(restaurants);

        return restaurants;
    }
}