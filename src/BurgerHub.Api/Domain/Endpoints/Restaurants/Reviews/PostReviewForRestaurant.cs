using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.Reviews;

public record PostReviewRequest(
    string RestaurantId,
    ReviewScoresRequest Scores);

public record ReviewScoresRequest(
    ReviewScore Texture,
    ReviewScore Taste,
    ReviewScore Visual);

public class PostReviewForRestaurant : BaseAsyncEndpoint
    .WithRequest<PostReviewRequest>
    .WithoutResponse
{
    private readonly IMongoCollection<Review> _reviewCollection;
    private readonly IMongoCollection<Restaurant> _restaurantCollection;

    public PostReviewForRestaurant(
        IMongoCollection<Review> reviewCollection,
        IMongoCollection<Restaurant> restaurantCollection)
    {
        _reviewCollection = reviewCollection;
        _restaurantCollection = restaurantCollection;
    }
    
    //TODO: change route to POST api/restaurants/{restaurantId}/reviews, so restaurantId is inferred from route: https://github.com/ffMathy/BurgerHub/issues/6
    [Authorize(Roles = AuthRoles.User)]
    [HttpPost("api/restaurants/reviews")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] PostReviewRequest request, 
        CancellationToken cancellationToken = new())
    {
        if(!ObjectId.TryParse(request.RestaurantId, out var restaurantId))
            return BadRequest("Invalid restaurant ID.");

        var doesRestaurantExist = await DoesRestaurantExistInMongoAsync(
            restaurantId, 
            cancellationToken);
        if (!doesRestaurantExist)
            return BadRequest("The provided restaurant does not exist.");
        
        await UpsertReviewInMongoAsync(
            request, 
            cancellationToken);

        return Ok();
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
        PostReviewRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredId();
        var restaurantId = ObjectId.Parse(request.RestaurantId);
        
        await _reviewCollection.UpdateOneAsync(
            review =>
                review.AuthorUserId == userId &&
                review.RestaurantId == restaurantId,
            Builders<Review>.Update
                .SetOnInsert(x => x.RestaurantId, restaurantId)
                .SetOnInsert(x => x.AuthorUserId, userId)
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