using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public class Review
{
    public ObjectId AuthorUserId { get; set; }
    public ReviewScores Scores { get; set; } = null!;
}

public class ReviewScores
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