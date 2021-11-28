using System.Web;
using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
    public override async Task<ActionResult> HandleAsync(
        [FromForm] PostPhotoRequest request, 
        CancellationToken cancellationToken = new())
    {
        await using var stream = new MemoryStream();
        await request.File.CopyToAsync(
            stream,
            cancellationToken);
        
        //TODO: upload memory stream to S3, or use pre-signed URLs

        var userId = User.GetRequiredId();
        var photo = new Photo()
        {
            Id = ObjectId.GenerateNewId(),
            AuthorUserId = userId,
            Url = $"https://example.com/{HttpUtility.UrlEncode(request.File.Name)}"
        };
        await _photoCollection.InsertOneAsync(
            photo,
            cancellationToken: cancellationToken);

        return Ok();
    }
}