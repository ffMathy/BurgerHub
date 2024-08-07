using Ardalis.ApiEndpoints;
using Ardalis.Result.AspNetCore;
using AutoMapper;
using BurgerHub.Api.Domain.Commands;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
    .WithResponse<Unit>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PostReviewForRestaurant(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    
    //TODO: change route to POST api/restaurants/{restaurantId}/reviews, so restaurantId is inferred from route: https://github.com/ffMathy/BurgerHub/issues/6
    
    [Authorize(Roles = AuthRoles.User)]
    [HttpPost("api/restaurants/reviews")]
    public override async Task<ActionResult<Unit>> HandleAsync(
        [FromBody] PostReviewRequest request, 
        CancellationToken cancellationToken = new())
    {
        if(!ObjectId.TryParse(request.RestaurantId, out var restaurantId))
            return BadRequest("Invalid restaurant ID.");

        var upsertReviewCommand = _mapper.Map<UpsertReviewCommand>(request);
        upsertReviewCommand = upsertReviewCommand with { RestaurantId = restaurantId, AuthorUserId = User.GetRequiredId() };

        var result = await _mediator.Send(upsertReviewCommand, cancellationToken);
        return this.ToActionResult(result);
    }
}
