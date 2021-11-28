using BurgerHub.Api.Domain.Models;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Domain.Queries;

public record LocationArguments(
    double Latitude,
    double Longitude);

public record GetNearbyRestaurantsQuery(
    LocationArguments Location,
    double RadiusInMeters,
    int Limit,
    int Offset) : IRequest<IReadOnlyList<Restaurant>>;

public class GetNearbyRestaurantsQueryHandler : IRequestHandler<GetNearbyRestaurantsQuery, IReadOnlyList<Restaurant>>
{
    private readonly IMongoCollection<Restaurant> _restaurantsCollection;

    public GetNearbyRestaurantsQueryHandler(
        IMongoCollection<Restaurant> restaurantsCollection)
    {
        _restaurantsCollection = restaurantsCollection;
    }
    
    public async Task<IReadOnlyList<Restaurant>> Handle(
        GetNearbyRestaurantsQuery request, 
        CancellationToken cancellationToken)
    {
        //TODO: introduce Ardalis.Result + FluentValidation for arguments? https://github.com/ffMathy/BurgerHub/issues/9
        
        if (request.Limit <= 0)
            return Array.Empty<Restaurant>();
        
        if (request.RadiusInMeters <= 0)
            return Array.Empty<Restaurant>();
        
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
}