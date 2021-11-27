using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants;

public record GetRestaurantsByLocationRequest(
    LocationRequest Location,
    long Offset,
    long Limit);
    
public record LocationRequest(
    decimal Latitude,
    decimal Longitude);
    
    
public record GetRestaurantsByLocationResponse(
    IEnumerable<RestaurantResponse> Restaurants);

public record RestaurantResponse(
    string Name,
    LocationResponse Location,
    OpeningTimeResponse[] OpeningTimes);

public record LocationResponse(
    decimal Latitude,
    decimal Longitude);

public record OpeningTimeResponse(
    DayOfWeek DayOfWeek,
    TimeOnly OpenTimeUtc,
    TimeOnly CloseTimeUtc);
    
    
public class GetRestaurantsByLocation : BaseAsyncEndpoint
    .WithRequest<GetRestaurantsByLocationRequest>
    .WithoutResponse
{
    private readonly IMongoCollection<Restaurant> _restaurantsCollection;

    public GetRestaurantsByLocation(
        IMongoCollection<Restaurant> restaurantsCollection)
    {
        _restaurantsCollection = restaurantsCollection;
    }
        
    [HttpGet]
    [AllowAnonymous]
    [HttpGet("api/restaurants/by-location")]
    public override Task<ActionResult> HandleAsync(
        GetRestaurantsByLocationRequest request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}