using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Models;

public record Restaurant(
    string Name,
    GeoJsonPoint<GeoJson2DGeographicCoordinates> Location,
    OpeningTime[] OpeningTimes);

public record OpeningTime(
    DayOfWeek DayOfWeek,
    TimeOnly OpenTimeUtc,
    TimeOnly CloseTimeUtc);