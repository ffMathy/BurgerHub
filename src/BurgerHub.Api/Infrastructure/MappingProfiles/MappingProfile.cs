using AutoMapper;
using BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BurgerHub.Api.Infrastructure.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PostListRestaurantsByLocationRequest, GetNearbyRestaurantsQuery>()
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationArguments(src.Location.Latitude, src.Location.Longitude)))
            .ForMember(dest => dest.RadiusInMeters, opt => opt.MapFrom(src => src.RadiusInMeters))
            .ForMember(dest => dest.Limit, opt => opt.MapFrom(src => src.Limit))
            .ForMember(dest => dest.Offset, opt => opt.MapFrom(src => src.Offset));

        CreateMap<Restaurant, RestaurantResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new LocationResponse(src.Location.Coordinates.Latitude, src.Location.Coordinates.Longitude)))
            .ForMember(dest => dest.DailyOpenTimes, opt => opt.MapFrom(src => src.DailyOpenTimes));

        CreateMap<LocationArguments, LocationRequest>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude));

        CreateMap<DailyOpeningTime, DailyOpeningTimeResponse>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.OpenAt, opt => opt.MapFrom(src => new TimeResponse(src.OpenAt.Hour, src.OpenAt.Minute)))
            .ForMember(dest => dest.ClosedAt, opt => opt.MapFrom(src => new TimeResponse(src.ClosedAt.Hour, src.ClosedAt.Minute)));
    }
}
