using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;

public class PostListRestaurantsByLocation : BaseAsyncEndpoint
    .WithRequest<PostListRestaurantsByLocationRequest>
    .WithResponse<PostListRestaurantsByLocationResponse>
{
    private readonly IMediator _mediator;

    public PostListRestaurantsByLocation(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("api/restaurants/by-location")]
    public override async Task<ActionResult<PostListRestaurantsByLocationResponse>> HandleAsync(
        [FromBody] PostListRestaurantsByLocationRequest request,
        CancellationToken cancellationToken = new())
    {
        var restaurants = await _mediator.Send(
            new GetNearbyRestaurantsQuery(
                new LocationArguments(
                    request.Location.Latitude,
                    request.Location.Longitude),
                request.RadiusInMeters,
                request.Limit,
                request.Offset),
            cancellationToken);
        return MapRestaurantsToResponse(restaurants);
    }

    private static PostListRestaurantsByLocationResponse MapRestaurantsToResponse(
        IEnumerable<Restaurant> restaurants)
    {
        return new PostListRestaurantsByLocationResponse(restaurants
            .Select(x => new RestaurantResponse(
                x.Name,
                x.Id.ToString(),
                MapLocationToLocationResponse(x.Location),
                x.DailyOpenTimes
                    .Select(MapOpeningTimeToOpeningTimeResponse)
                    .ToArray())));
    }

    private static LocationResponse MapLocationToLocationResponse(
        GeoJsonPoint<GeoJson2DGeographicCoordinates> location)
    {
        return new LocationResponse(
            location.Coordinates.Latitude,
            location.Coordinates.Longitude);
    }

    private static DailyOpeningTimeResponse MapOpeningTimeToOpeningTimeResponse(
        DailyOpeningTime dailyOpeningTime)
    {
        return new DailyOpeningTimeResponse(
            dailyOpeningTime.DayOfWeek,
            MapTimeToTimeResponse(dailyOpeningTime.OpenAt),
            MapTimeToTimeResponse(dailyOpeningTime.ClosedAt));
    }

    private static TimeResponse MapTimeToTimeResponse(Time time)
    {
        return new TimeResponse(
            time.Hour,
            time.Minute);
    }
}