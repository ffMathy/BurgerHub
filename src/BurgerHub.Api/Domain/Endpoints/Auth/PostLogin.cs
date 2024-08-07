using System.Security.Claims;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries.Users;
using BurgerHub.Api.Infrastructure.Security.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

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
    private readonly IMapper _mapper;
    
    private const string AuthenticationFailureMessage = "Invalid email or password";

    public PostLogin(
        IJwtTokenFactory jwtTokenFactory,
        IMediator mediator,
        IMapper mapper)
    {
        _jwtTokenFactory = jwtTokenFactory;
        _mediator = mediator;
        _mapper = mapper;
    }
    
    [AllowAnonymous]
    [HttpPost("api/auth/login")]
    public override async Task<ActionResult<PostLoginResponse>> HandleAsync(
        PostLoginRequest request, 
        CancellationToken cancellationToken = new())
    {
        var query = _mapper.Map<GetUserByCredentialsQuery>(request);
        var user = await _mediator.Send(query, cancellationToken);
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
