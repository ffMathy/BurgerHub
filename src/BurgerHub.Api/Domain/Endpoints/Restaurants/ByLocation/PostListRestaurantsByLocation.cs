using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;

public class PostListRestaurantsByLocation : BaseAsyncEndpoint
    .WithRequest<PostListRestaurantsByLocationRequest>
    .WithResponse<PostListRestaurantsByLocationResponse>
{
    private readonly IMongoCollection<Restaurant> _restaurantsCollection;

    public PostListRestaurantsByLocation(
        IMongoCollection<Restaurant> restaurantsCollection)
    {
        _restaurantsCollection = restaurantsCollection;
    }

    [AllowAnonymous]
    [HttpPost("api/restaurants/by-location")]
    public override async Task<ActionResult<PostListRestaurantsByLocationResponse>> HandleAsync(
        [FromBody] PostListRestaurantsByLocationRequest request,
        CancellationToken cancellationToken = new())
    {
        var restaurants = await FetchNearbyRestaurantsFromMongoAsync(request, cancellationToken);
        return MapRestaurantsToResponse(restaurants);
    }

    private async Task<List<Restaurant>> FetchNearbyRestaurantsFromMongoAsync(PostListRestaurantsByLocationRequest request, CancellationToken cancellationToken)
    {
        return await _restaurantsCollection
            .Find(Builders<Restaurant>.Filter.Near(
                x => x.Location,
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(
                        request.Location.Longitude,
                        request.Location.Latitude)),
                request.RadiusInMeters))
            .Skip(request.Offset)
            .Limit(request.Limit)
            .ToListAsync(cancellationToken);
    }

    private static PostListRestaurantsByLocationResponse MapRestaurantsToResponse(List<Restaurant> restaurants)
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

    private static LocationResponse MapLocationToLocationResponse(GeoJsonPoint<GeoJson2DGeographicCoordinates> location)
    {
        return new LocationResponse(
            location.Coordinates.Latitude,
            location.Coordinates.Longitude);
    }

    private static DailyOpeningTimeResponse MapOpeningTimeToOpeningTimeResponse(DailyOpeningTime dailyOpeningTime)
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