using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Models;

public class Restaurant
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = null!;
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; } = null!;
    public DailyOpeningTime[] DailyOpenTimes { get; set; } = null!;
}

public class DailyOpeningTime
{
    public DayOfWeek DayOfWeek { get; set; }
    public Time OpenAt { get; set; } = null!;
    public Time ClosedAt { get; set; } = null!;
}

public class Time
{
    public int Hour { get; set; }
    public int Minute { get; set; }
}