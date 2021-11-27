using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models;

public class User
{
    public ObjectId Id { get; set; }
    public byte[] EncryptedEmail { get; set; } = null!;
    public byte[] HashedPassword { get; set; } = null!;
}