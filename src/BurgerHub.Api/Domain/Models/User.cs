using MongoDB.Bson;

namespace BurgerHub.Api.Domain.Models
{
    public record User(
        ObjectId Id,
        byte[] EncryptedEmail,
        byte[] HashedPassword);
}