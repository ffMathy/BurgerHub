﻿using Bogus;
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
    private readonly IEncryptionHelper _encryptionHelper;

    public FakerFactory(
        IEncryptionHelper encryptionHelper)
    {
        _encryptionHelper = encryptionHelper;
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
                x => _encryptionHelper
                    .EncryptAsync(
                        x.Person.Email,
                        withoutSalt: true)
                    .Result)
            .RuleFor(
                x => x.HashedPassword,
                x => _encryptionHelper
                    .Hash(x.Internet.Password(
                        length: 10,
                        memorable: true)));
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