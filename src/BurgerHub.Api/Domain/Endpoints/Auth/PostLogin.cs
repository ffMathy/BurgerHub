using System.Security.Claims;
using Ardalis.ApiEndpoints;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries;
using BurgerHub.Api.Domain.Queries.Users;
using BurgerHub.Api.Infrastructure.Security.Auth;
using BurgerHub.Api.Infrastructure.Security.Encryption;
using MediatR;
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
    private readonly IMediator _mediator;
    
    private const string AuthenticationFailureMessage = "Invalid email or password";

    public PostLogin(
        IJwtTokenFactory jwtTokenFactory,
        IMediator mediator)
    {
        _jwtTokenFactory = jwtTokenFactory;
        _mediator = mediator;
    }
    
    [AllowAnonymous]
    [HttpPost("api/auth/login")]
    public override async Task<ActionResult<PostLoginResponse>> HandleAsync(
        PostLoginRequest request, 
        CancellationToken cancellationToken = new())
    {
        var user = await _mediator.Send(
            new GetUserByCredentialsQuery(
                request.Email,
                request.Password),
            cancellationToken);
        if (user == null)
            return Unauthorized(AuthenticationFailureMessage);
        
        var bearerToken = CreateTokenForUser(user);
        return new PostLoginResponse(bearerToken);
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