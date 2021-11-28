using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Commands;
using BurgerHub.Api.Infrastructure.AspNet;
using BurgerHub.Api.Infrastructure.Security.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BurgerHub.Api.Domain.Endpoints.Photos;

public record PostPhotoRequest(
    IFormFile File);

public class PostPhoto : BaseAsyncEndpoint
    .WithRequest<PostPhotoRequest>
    .WithoutResponse
{
    private readonly IMediator _mediator;

    public PostPhoto(
        IMediator mediator)
    {
        _mediator = mediator;
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

        await _mediator.Send(
            new UploadPhotoCommand(
                User.GetRequiredId(),
                stream.ToArray()),
            cancellationToken);

        return Ok();
    }
}