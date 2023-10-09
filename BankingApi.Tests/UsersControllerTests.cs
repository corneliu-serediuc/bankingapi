using BankingApi.Controllers;
using BankingApi.Data;
using BankingApi.Entities;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BankingApi.Tests;

public class UsersControllerTests
{
    [Fact]
    public void GetUser_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var userId = "user123";
        var user = new User { Id = userId, Name = "Test User", Email = "test@example.com" };
        var dataStore = new Mock<IDataStore>();
        dataStore.Setup(d => d.Users.Get(userId)).Returns(user);
        var controller = new UsersController(dataStore.Object);

        // Act
        var result = controller.GetUser(userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public void GetUser_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistentUser";
        var dataStore = new Mock<IDataStore>();
        dataStore.Setup(d => d.Users.Get(userId)).Returns(It.IsAny<User>());
        var controller = new UsersController(dataStore.Object);

        // Act
        var result = controller.GetUser(userId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsType<ErrorResponse>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void CreateUser_ReturnsOkResult_WhenUserIsCreated()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest { Name = "New User", Email = "newuser@example.com" };
        var dataStore = new Mock<IDataStore>();
        dataStore.Setup(d => d.Users.Set(It.IsAny<string>(), It.IsAny<User>())).Returns(true);
        var controller = new UsersController(dataStore.Object);

        // Act
        var result = controller.CreateUser(createUserRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsType<User>(okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public void CreateUser_ReturnsConflictResult_WhenUserAlreadyExists()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest { Name = "Existing User", Email = "existinguser@example.com" };
        var dataStore = new Mock<IDataStore>();
        dataStore.Setup(d => d.Users.Set(It.IsAny<string>(), It.IsAny<User>())).Returns(false);
        var controller = new UsersController(dataStore.Object);

        // Act
        var result = controller.CreateUser(createUserRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
        var conflictResult = result as ConflictObjectResult;
        Assert.IsType<ErrorResponse>(conflictResult.Value);
        Assert.Equal(409, conflictResult.StatusCode);
    }
}
