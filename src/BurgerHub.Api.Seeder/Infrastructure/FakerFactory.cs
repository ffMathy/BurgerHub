using Bogus;
using Bogus.DataSets;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Encryption;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

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
        return new Faker<Review>()
            .StrictMode(true)
            .RuleFor(
                x => x.AuthorUserId,
                ObjectId.GenerateNewId)
            .RuleForType(
                typeof(ReviewScores),
                x => new ReviewScores()
                {
                    Taste = x.Random.Enum<ReviewScore>(),
                    Texture = x.Random.Enum<ReviewScore>(),
                    Visual = x.Random.Enum<ReviewScore>()
                });
    }

    public Faker<Photo> CreatePhotoFaker()
    {
        return new Faker<Photo>()
            .StrictMode(true)
            .RuleFor(
                x => x.Id,
                ObjectId.GenerateNewId)
            .RuleFor(
                x => x.Url,
                x => x.Image.PlaceImgUrl(
                    category: PlaceImgCategory.Architecture))
            .Ignore(
                x => x.AuthorUserId);
    }

    public Faker<Restaurant> CreateRestaurantFaker()
    {
        return new Faker<Restaurant>()
            .StrictMode(true)
            .RuleFor(
                x => x.Name,
                x => x.Company.CompanyName())
            .RuleFor(
                x => x.Location,
                x => new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(
                        x.Address.Longitude(),
                        x.Address.Latitude())))
            .RuleFor(
                x => x.OpeningTimes,
                x => x.Random.ArrayElements(x.Random
                    .EnumValues<DayOfWeek>()
                    .Select(dayOfWeek => new OpeningTime() {
                        DayOfWeek = dayOfWeek,
                        OpenTimeUtc = x.Date.BetweenTimeOnly(
                            new TimeOnly(8, 0),
                            new TimeOnly(11, 0)),
                        CloseTimeUtc = x.Date.BetweenTimeOnly(
                            new TimeOnly(16, 0),
                            new TimeOnly(21, 0))
                    })
                    .ToArray()));
    }
}