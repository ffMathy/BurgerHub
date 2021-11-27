using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.Reviews;

public record PostReviewRequest(
    [FromRoute] string RestaurantId,
    [FromBody] ReviewScoresRequest Scores);

public record ReviewScoresRequest(
    ReviewScore? Texture,
    ReviewScore? Taste,
    ReviewScore? Visual);

public class PostReviewForRestaurant : BaseAsyncEndpoint
    .WithRequest<PostReviewRequest>
    .WithoutResponse
{
    [HttpPost("api/restaurants/{RestaurantId}/review")]
    public override Task<ActionResult> HandleAsync(
        [FromRoute] PostReviewRequest request, 
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}