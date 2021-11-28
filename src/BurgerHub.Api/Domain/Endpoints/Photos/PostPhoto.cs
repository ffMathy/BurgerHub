using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Endpoints.Photos;

public record PostPhotoRequest(
    IFormFile File);

public class PostPhoto : BaseAsyncEndpoint
    .WithRequest<PostPhotoRequest>
    .WithoutResponse
{
    private readonly IMongoCollection<Photo> _photoCollection;

    public PostPhoto(
        IMongoCollection<Photo> photoCollection)
    {
        _photoCollection = photoCollection;
    }
    
    [HttpPost("api/photos")]
    [Authorize(Roles = AuthRoles.User)]
    public override Task<ActionResult> HandleAsync(
        PostPhotoRequest request, 
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}