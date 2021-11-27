using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public record Photo(
    ObjectId Id,
    string Url)
{
    public ObjectId AuthorUserId { get; set; }
};