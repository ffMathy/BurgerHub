using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public class Photo
{
    public ObjectId AuthorUserId { get; set; }
    public ObjectId Id { get; set; }
    public string Url { get; set; } = null!;
};