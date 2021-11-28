using BurgerHub.Api.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BurgerHub.Api.Domain.Queries.Users;

public record GetUserByCredentialsQuery(
    string Email,
    string Password) : IRequest<User?>;

public class GetUserByCredentialsQueryHandler : IRequestHandler<GetUserByCredentialsQuery, User?>
{
    private readonly IMediator _mediator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public GetUserByCredentialsQueryHandler(
        IMediator mediator,
        IPasswordHasher<User> passwordHasher)
    {
        _mediator = mediator;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<User?> Handle(
        GetUserByCredentialsQuery request, 
        CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(
            new GetUserByEmailQuery(request.Email),
            cancellationToken);
        if (user == null)
            return null;
        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.HashedPassword,
            request.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        return user;
    }
}