using Ardalis.ApiEndpoints;
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
    [Authorize]
    public override Task<ActionResult> HandleAsync(PostPictureRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}