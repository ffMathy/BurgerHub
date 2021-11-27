using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Models;

public class Restaurant
{
    public string Name { get; set; } = null!;
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = null!;
    public OpeningTime[] OpeningTimes { get; set; } = null!;
}

public class OpeningTime
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTimeUtc { get; set; }
    public TimeOnly CloseTimeUtc { get; set; }
}