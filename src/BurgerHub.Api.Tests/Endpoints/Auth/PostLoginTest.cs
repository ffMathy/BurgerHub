using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BurgerHub.Api.Domain.Endpoints.Auth;
using BurgerHub.Api.Domain.Models;
using BurgerHub.Api.Domain.Queries.Users;
using BurgerHub.Api.Infrastructure.Security.Auth;
using BurgerHub.Api.Tests.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using NSubstitute;

namespace BurgerHub.Api.Tests.Endpoints.Auth;

[TestClass]
public class PostLoginTest
{
    [TestMethod]
    [TestCategory(TestCategories.UnitCategory)]
    public async Task HandleAsync_NoUserFound_ReturnsUnauthorizedStatus()
    {
        //Arrange
        var fakeMediator = Substitute.For<IMediator>();
        fakeMediator
            .Send(Arg.Any<GetUserByCredentialsQuery>())
            .Returns((User?)null);

        var handler = new PostLogin(
            Substitute.For<IJwtTokenFactory>(),
            fakeMediator);

        //Act
        var result = await handler.HandleAsync(
            new PostLoginRequest(
                "dummy",
                "dummy"));

        //Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory(TestCategories.UnitCategory)]
    public async Task HandleAsync_UserFound_ReturnsResponseWithGeneratedJwtToken()
    {
        //Arrange
        var userId = ObjectId.GenerateNewId();
        var userName = "some-name";

        var fakeMediator = Substitute.For<IMediator>();
        fakeMediator
            .Send(Arg.Any<GetUserByCredentialsQuery>())
            .Returns(new User()
            {
                Id = userId,
                Name = userName
            });

        var fakeJwtTokenFactory = Substitute.For<IJwtTokenFactory>();
        fakeJwtTokenFactory
            .Create(Arg.Is<Claim[]>(claims =>
                claims.Single(x => x.Type == JwtRegisteredClaimNames.Sub).Value == userId.ToString() &&
                claims.Single(x => x.Type == JwtRegisteredClaimNames.Name).Value == userName &&
                claims.Single(x => x.Type == ClaimTypes.Role).Value == AuthRoles.User))
            .Returns("some-token");

        var handler = new PostLogin(
            fakeJwtTokenFactory,
            fakeMediator);

        //Act
        var result = await handler.HandleAsync(
            new PostLoginRequest(
                "dummy",
                "dummy"));

        //Assert
        Assert.AreEqual("some-token", result.Value?.BearerToken);
    }
}