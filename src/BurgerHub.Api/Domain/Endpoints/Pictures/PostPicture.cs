using Ardalis.ApiEndpoints;
using BurgerHub.Api.Infrastructure.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerHub.Api.Domain.Endpoints.Pictures;

public record PostPictureRequest(
    IFormFile File);

public class PostPicture : BaseAsyncEndpoint
    .WithRequest<PostPictureRequest>
    .WithoutResponse
{
    public PostPicture()
    {
        
    }
    
    [HttpPost("api/pictures")]
    [Authorize(Roles = AuthRoles.User)]
    public override Task<ActionResult> HandleAsync(PostPictureRequest request, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}