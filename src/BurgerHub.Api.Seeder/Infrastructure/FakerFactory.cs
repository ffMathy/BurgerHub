using Bogus;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using MongoDB.Bson;

namespace BurgerHub.Api.Seeder.Infrastructure;

public interface IFakerFactory
{
    Faker<User> CreateUserFaker();
    Faker<Review> CreateReviewFaker();
    Faker<Photo> CreatePhotoFaker();
    Faker<Restaurant> CreateRestaurantFaker();
}

public class FakerFactory : IFakerFactory
{
    private readonly IAesEncryptionHelper _aesEncryptionHelper;

    public FakerFactory(
        IAesEncryptionHelper aesEncryptionHelper)
    {
        _aesEncryptionHelper = aesEncryptionHelper;
    }
    
    public Faker<User> CreateUserFaker()
    {
        return new Faker<User>()
            .StrictMode(true)
            .RuleFor(
                x => x.Id,
                ObjectId.GenerateNewId)
            .RuleFor(
                x => x.EncryptedEmail,
                x => _aesEncryptionHelper
                    .EncryptAsync(
                        x.Person.Email,
                        withoutSalt: true)
                    .Result)
            .RuleFor(
                x => x.HashedPassword,
                x => _aesEncryptionHelper
                    .EncryptAsync(
                        x.Internet.Password(
                            length: 10,
                            memorable: true),
                        withoutSalt: true)
                    .Result);
    }

    public Faker<Review> CreateReviewFaker()
    {
        throw new NotImplementedException();
    }

    public Faker<Photo> CreatePhotoFaker()
    {
        throw new NotImplementedException();
    }

    public Faker<Restaurant> CreateRestaurantFaker()
    {
        throw new NotImplementedException();
    }
}