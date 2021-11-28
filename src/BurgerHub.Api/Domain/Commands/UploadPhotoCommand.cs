using BurgerHub.Api.Domain.Models;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Commands;

public record UploadPhotoCommand(
    ObjectId AuthorUserId,
    byte[] Bytes) : IRequest;

public class UploadPhotoCommandHandler : IRequestHandler<UploadPhotoCommand>
{
    private readonly IMongoCollection<Photo> _photoCollection;

    public UploadPhotoCommandHandler(
        IMongoCollection<Photo> photoCollection)
    {
        _photoCollection = photoCollection;
    }
    
    public async Task<Unit> Handle(
        UploadPhotoCommand request, 
        CancellationToken cancellationToken)
    {
        //TODO: upload memory stream to S3, or use pre-signed URLs
        
        var photo = new Photo()
        {
            Id = ObjectId.GenerateNewId(),
            AuthorUserId = request.AuthorUserId,
            Url = $"https://example.com/{Guid.NewGuid()}"
        };
        await _photoCollection.InsertOneAsync(
            photo,
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}