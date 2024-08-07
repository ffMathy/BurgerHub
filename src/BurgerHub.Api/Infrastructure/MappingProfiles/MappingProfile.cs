using AutoMapper;
using BurgerHub.Api.Domain.Commands;
using BurgerHub.Api.Domain.Endpoints.Auth;
using BurgerHub.Api.Domain.Endpoints.Photos;
using BurgerHub.Api.Domain.Endpoints.Restaurants.ByLocation;
using BurgerHub.Api.Domain.Endpoints.Restaurants.Reviews;

namespace BurgerHub.Api.Infrastructure.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PostListRestaurantsByLocationRequest, GetNearbyRestaurantsQuery>();
        CreateMap<Restaurant, RestaurantResponse>();
        CreateMap<Location, LocationResponse>();
        CreateMap<DailyOpeningTime, DailyOpeningTimeResponse>();
        CreateMap<Time, TimeResponse>();

        CreateMap<PostReviewRequest, UpsertReviewCommand>();
        CreateMap<ReviewScoresRequest, ReviewScoresArgument>();

        CreateMap<PostLoginRequest, GetUserByCredentialsQuery>();
        CreateMap<User, PostLoginResponse>();

        CreateMap<PostPhotoRequest, UploadPhotoCommand>();
    }
}
