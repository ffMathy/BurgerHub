using Ardalis.Result;
using BurgerHub.Api.Domain.Models;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Commands;

public record ReviewScoresArgument(
    ReviewScore Texture,
    ReviewScore Taste,
    ReviewScore Visual);

public record UpsertReviewCommand(
    ObjectId RestaurantId,
    ObjectId AuthorUserId,
    ReviewScoresArgument Scores) : IRequest<Result<Unit>>;

public class UpsertReviewCommandHandler : IRequestHandler<UpsertReviewCommand, Result<Unit>>
{
    private readonly IMongoCollection<Review> _reviewCollection;
    private readonly IMongoCollection<Restaurant> _restaurantCollection;

    private const string RestaurantDoesNotExistErrorCode = "RESTAURANT_DOES_NOT_EXIST";

    public UpsertReviewCommandHandler(
        IMongoCollection<Review> reviewCollection, 
        IMongoCollection<Restaurant> restaurantCollection)
    {
        _reviewCollection = reviewCollection;
        _restaurantCollection = restaurantCollection;
    }

    public async Task<Result<Unit>> Handle(
        UpsertReviewCommand request, 
        CancellationToken cancellationToken)
    {
        var doesRestaurantExist = await DoesRestaurantExistInMongoAsync(
            request.RestaurantId, 
            cancellationToken);
        if (!doesRestaurantExist)
        {
            return Result<Unit>.Invalid(new ()
            {
                new ValidationError()
                {
                    Severity = ValidationSeverity.Error,
                    Identifier = RestaurantDoesNotExistErrorCode
                }
            });
        }

        await UpsertReviewInMongoAsync(
            request, 
            cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }

    private async Task<bool> DoesRestaurantExistInMongoAsync(
        ObjectId restaurantId, 
        CancellationToken cancellationToken)
    {
        var matchCount = await _restaurantCollection
            .Find(x => x.Id == restaurantId)
            .Limit(1)
            .CountDocumentsAsync(cancellationToken);
        return matchCount > 0;
    }

    private async Task UpsertReviewInMongoAsync(
        UpsertReviewCommand request, 
        CancellationToken cancellationToken)
    {
        await _reviewCollection.UpdateOneAsync(
            review =>
                review.AuthorUserId == request.AuthorUserId &&
                review.RestaurantId == request.RestaurantId,
            Builders<Review>.Update
                .SetOnInsert(x => x.RestaurantId, request.RestaurantId)
                .SetOnInsert(x => x.AuthorUserId, request.AuthorUserId)
                .Set(x => x.Scores.Taste, request.Scores.Taste)
                .Set(x => x.Scores.Texture, request.Scores.Texture)
                .Set(x => x.Scores.Visual, request.Scores.Visual),
            new UpdateOptions()
            {
                IsUpsert = true
            },
            cancellationToken);
    }
}