using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public record Photo(
    ObjectId Id,
    ObjectId AuthorUserId,
    string Url);