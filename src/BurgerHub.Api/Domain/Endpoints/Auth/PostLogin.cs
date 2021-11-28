using System.Security.Claims;
using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Infrastructure.Security.Auth;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Driver;

namespace BurgerHub.Api.Domain.Endpoints.Auth;

public record PostLoginRequest(
    string Email,
    string Password);

public record PostLoginResponse(
    string BearerToken);

public class PostLogin : BaseAsyncEndpoint
    .WithRequest<PostLoginRequest>
    .WithResponse<PostLoginResponse>
{
    private readonly IJwtTokenFactory _jwtTokenFactory;
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IMongoCollection<User> _userCollection;
    
    private const string AuthenticationFailureMessage = "Invalid email or password";

    public PostLogin(
        IJwtTokenFactory jwtTokenFactory,
        IEncryptionHelper encryptionHelper,
        IPasswordHasher<User> passwordHasher,
        IMongoCollection<User> userCollection)
    {
        _jwtTokenFactory = jwtTokenFactory;
        _encryptionHelper = encryptionHelper;
        _passwordHasher = passwordHasher;
        _userCollection = userCollection;
    }
    
    [AllowAnonymous]
    [HttpPost("api/auth/login")]
    public override async Task<ActionResult<PostLoginResponse>> HandleAsync(
        PostLoginRequest request, 
        CancellationToken cancellationToken = new())
    {
        var user = await FetchUserByEmailFromMongoAsync(
            request.Email, 
            cancellationToken);
        if (user == null)
            return Unauthorized(AuthenticationFailureMessage);

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.HashedPassword,
            request.Password);
        if(passwordVerificationResult == PasswordVerificationResult.Failed)
            return Unauthorized(AuthenticationFailureMessage);
        
        var bearerToken = CreateTokenForUser(user);
        return new PostLoginResponse(bearerToken);
    }

    private async Task<User?> FetchUserByEmailFromMongoAsync(
        string plainTextEmailAddress, 
        CancellationToken cancellationToken)
    {
        var encryptedEmailAddress = await _encryptionHelper.EncryptAsync(
            plainTextEmailAddress,
            withoutSalt: true);
        return await _userCollection
            .Find(x => x.EncryptedEmail == encryptedEmailAddress)
            .SingleOrDefaultAsync(cancellationToken);
    }

    private string CreateTokenForUser(User user)
    {
        var bearerToken = _jwtTokenFactory.Create(new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),
            new Claim(
                JwtRegisteredClaimNames.Name,
                user.Name),
            new Claim(
                ClaimTypes.Role,
                AuthRoles.User)
        });
        return bearerToken;
    }
}