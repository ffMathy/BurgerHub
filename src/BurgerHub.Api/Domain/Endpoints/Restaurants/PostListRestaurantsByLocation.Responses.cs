namespace BurgerHub.Api.Domain.Endpoints.Restaurants;

public record PostListRestaurantsByLocationResponse(
    IEnumerable<RestaurantResponse> Restaurants);

public record RestaurantResponse(
    string Name,
    string Id,
    LocationResponse Location,
    DailyOpeningTimeResponse[] DailyOpenTimes);

public record LocationResponse(
    double Latitude,
    double Longitude);

public record DailyOpeningTimeResponse(
    DayOfWeek DayOfWeek,
    TimeResponse OpenAt,
    TimeResponse ClosedAt);

public record TimeResponse(
    int Hour,
    int Minute);