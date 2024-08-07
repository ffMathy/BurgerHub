using Ardalis.ApiEndpoints;
using AutoMapper;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;

public class PostListRestaurantsByLocation : BaseAsyncEndpoint
    .WithRequest<PostListRestaurantsByLocationRequest>
    .WithResponse<PostListRestaurantsByLocationResponse>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public PostListRestaurantsByLocation(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost("api/restaurants/by-location")]
    public override async Task<ActionResult<PostListRestaurantsByLocationResponse>> HandleAsync(
        [FromBody] PostListRestaurantsByLocationRequest request,
        CancellationToken cancellationToken = new())
    {
        var query = _mapper.Map<GetNearbyRestaurantsQuery>(request);
        var restaurants = await _mediator.Send(query, cancellationToken);
        var response = _mapper.Map<PostListRestaurantsByLocationResponse>(restaurants);
        return response;
    }
}
