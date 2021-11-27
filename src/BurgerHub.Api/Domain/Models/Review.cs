using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public record Review(
    ReviewScores Scores)
{
    public ObjectId AuthorUserId { get; set; }
};

public record ReviewScores
{
    public ReviewScore? Texture { get; set; }
    public ReviewScore? Taste { get; set; }
    public ReviewScore? Visual { get; set; }
}

public enum ReviewScore
{
    Terrible = -2,
    Bad = -1,
    Neutral = 0,
    Good = 1,
    Great = 2
}