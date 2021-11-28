using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    public PostReviewForRestaurant(
        IMongoCollection<Review> reviewCollection)
    {
        _reviewCollection = reviewCollection;
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
        
        var userId = User.GetRequiredId();

        await _reviewCollection.UpdateOneAsync(
            review =>
                review.AuthorUserId == userId &&
                review.RestaurantId == restaurantId,
            Builders<Review>.Update
                .Set(x => x.Scores.Taste, request.Scores.Taste)
                .Set(x => x.Scores.Texture, request.Scores.Texture)
                .Set(x => x.Scores.Visual, request.Scores.Visual),
            new UpdateOptions()
            {
                IsUpsert = true
            },
            cancellationToken);

        return Ok();
    }
}