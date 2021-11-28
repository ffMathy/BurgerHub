using System.ComponentModel.DataAnnotations;

namespace BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;

public record PostListRestaurantsByLocationRequest(
    LocationRequest Location,
    int Offset,
    [Range(1, 100)] int Limit,
    [Range(1, int.MaxValue)] long RadiusInMeters);

public record LocationRequest(
    [Range(-90, 90)] double Latitude,
    [Range(-180, 180)]double Longitude);