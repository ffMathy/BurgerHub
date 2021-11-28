using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using MediatR;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Queries.Users;

public record GetUserByEmailQuery(
    string Email) : IRequest<User?>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
{
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly IMongoCollection<User> _usersCollection;

    public GetUserByEmailQueryHandler(
        IEncryptionHelper encryptionHelper,
        IMongoCollection<User> usersCollection)
    {
        _encryptionHelper = encryptionHelper;
        _usersCollection = usersCollection;
    }
    
    public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var encryptedEmailAddress = await _encryptionHelper.EncryptAsync(
            request.Email,
            withoutSalt: true);
        return await _usersCollection
            .Find(x => x.EncryptedEmail == encryptedEmailAddress)
            .SingleOrDefaultAsync(cancellationToken);
    }
}