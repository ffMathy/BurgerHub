using Bogus;
using Bogus.DataSets;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using Microsoft.AspNetCore.Identity;
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
    public Faker<User> CreateUserFaker()
    {
        return new Faker<User>()
            .StrictMode(true)
            .RuleFor(
                x => x.Id,
                ObjectId.GenerateNewId)
            .RuleFor(
                x => x.Name,
                x => x.Name.FirstName())
            .Ignore(x => x.EncryptedEmail)
            .Ignore(x => x.HashedPassword);
    }

    public Faker<Review> CreateReviewFaker()
    {
        return new Faker<Review>()
            .StrictMode(true)
            .Ignore(x => x.AuthorUserId)
            .Ignore(x => x.RestaurantId)
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
            .RuleFor(
                x => x.Id,
                ObjectId.GenerateNewId)
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
                x => x.DailyOpenTimes,
                x => Enum.GetValues<DayOfWeek>()
                    .Select(dayOfWeek => new DailyOpeningTime() {
                        DayOfWeek = dayOfWeek,
                        OpenAt = new Time() {
                            Hour = x.Random.Number(8, 11)
                        },
                        ClosedAt = new Time() {
                            Hour = x.Random.Number(16, 21)
                        }
                    })
                    .OrderBy(y => y.DayOfWeek)
                    .ToArray());
    }
}