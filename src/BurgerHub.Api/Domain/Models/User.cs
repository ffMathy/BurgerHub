using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public class User
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string EncryptedEmail { get; set; } = null!;
    public string HashedPassword { get; set; } = null!;
}