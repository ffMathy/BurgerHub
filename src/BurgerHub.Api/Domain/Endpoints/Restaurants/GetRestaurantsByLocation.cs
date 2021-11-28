using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants;


public record GetRestaurantsByLocationRequest(
    [FromQuery] LocationRequest Location,
    [FromQuery] int Offset,
    [FromQuery] int Limit,
    [FromQuery] long RadiusInMeters);

public record LocationRequest(
    [FromQuery] double Latitude,
    [FromQuery] double Longitude);


public record GetRestaurantsByLocationResponse(
    IEnumerable<RestaurantResponse> Restaurants);

public record RestaurantResponse(
    string Name,
    string Id,
    LocationResponse Location,
    OpeningTimeResponse[] OpeningTimes);

public record LocationResponse(
    double Latitude,
    double Longitude);

public record OpeningTimeResponse(
    DayOfWeek DayOfWeek,
    TimeResponse OpenTime,
    TimeResponse CloseTime);

public record TimeResponse(
    int Hour,
    int Minute);


public class GetRestaurantsByLocation : BaseAsyncEndpoint
    .WithRequest<GetRestaurantsByLocationRequest>
    .WithResponse<GetRestaurantsByLocationResponse>
{
    private readonly IMongoCollection<Restaurant> _restaurantsCollection;

    public GetRestaurantsByLocation(
        IMongoCollection<Restaurant> restaurantsCollection)
    {
        _restaurantsCollection = restaurantsCollection;
    }

    [AllowAnonymous]
    [HttpGet("api/restaurants/by-location")]
    public override async Task<ActionResult<GetRestaurantsByLocationResponse>> HandleAsync(
        [FromQuery] GetRestaurantsByLocationRequest request,
        CancellationToken cancellationToken = new())
    {
        if (request.Limit == 0)
            return BadRequest("Limit must be larger than 0.");

        if (request.Limit > 100)
            return BadRequest("Limit must be below 100.");
        
        if(request.RadiusInMeters == 0)
            return BadRequest("Radius must be larger than 0.");
        
        var restaurants = await _restaurantsCollection
            .Find(Builders<Restaurant>.Filter.Near(
                x => x.Location,
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(
                        request.Location.Longitude,
                        request.Location.Latitude))))
            .Skip(request.Offset)
            .Limit(request.Limit)
            .ToListAsync(cancellationToken);
        
        return new GetRestaurantsByLocationResponse(restaurants
            .Select(x => new RestaurantResponse(
                x.Name,
                x.Id.ToString(),
                MapLocationToLocationResponse(x.Location),
                x.OpeningTimes
                    .Select(MapOpeningTimeToOpeningTimeResponse)
                    .ToArray())));
    }

    private static LocationResponse MapLocationToLocationResponse(GeoJsonPoint<GeoJson2DGeographicCoordinates> location)
    {
        return new LocationResponse(
            location.Coordinates.Latitude,
            location.Coordinates.Longitude);
    }

    private static OpeningTimeResponse MapOpeningTimeToOpeningTimeResponse(OpeningTime openingTime)
    {
        return new OpeningTimeResponse(
            openingTime.DayOfWeek,
            MapTimeToTimeResponse(openingTime.OpenTime),
            MapTimeToTimeResponse(openingTime.CloseTime));
    }

    private static TimeResponse MapTimeToTimeResponse(Time time)
    {
        return new TimeResponse(
            time.Hour,
            time.Minute);
    }
}