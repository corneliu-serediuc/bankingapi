using BankingApi.Controllers;
using BankingApi.Data;
using BankingApi.Entities;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace BankingApi.Tests;

public class AccountsControllerTests
{
    [Fact]
    public void GetAccount_ReturnsOkResult_WithUserAccounts()
    {
        // Arrange
        var userId = "user123";
        var user = new User { Id = userId, Name = "Test User", Email = "test@example.com" };
        var account1 = new Account { Id = "account1", UserId = userId, Balance = 100.0M };
        var account2 = new Account { Id = "account2", UserId = userId, Balance = 200.0M };

        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Users.ContainsKey(userId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.GetAll()).Returns(new List<Account> { account1, account2 });
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.GetAccount(userId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsType<List<Account>>(okResult.Value);
        var accounts = okResult.Value as List<Account>;
        Assert.Equal(2, accounts.Count);
    }

    [Fact]
    public void GetAccount_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "nonexistentUser";
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Users.ContainsKey(userId)).Returns(false);
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.GetAccount(userId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("User not found.", (notFoundResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void CreateAccountForUser_ReturnsOkResult_WhenAccountIsCreated()
    {
        // Arrange
        var userId = "user123";
        var createUserRequest = new CreateAccountRequest { Balance = 150.0M };
        var account = new Account { UserId = userId, Balance = createUserRequest.Balance };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Users.ContainsKey(userId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Set(It.IsAny<string>(), It.IsAny<Account>())).Returns(true);
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.CreateAccountForUser(userId, createUserRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult.Value);
        Assert.IsType<Account>(okResult.Value);
    }

    [Fact]
    public void CreateAccountForUser_ReturnsBadRequestResult_WhenBalanceIsInvalid()
    {
        // Arrange
        var userId = "user123";
        var createUserRequest = new CreateAccountRequest { Balance = 50.0M }; // Invalid balance
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Users.ContainsKey(userId)).Returns(true);
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.CreateAccountForUser(userId, createUserRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.Equal("Initial balance must be at least $100.", (badRequestResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void CreateAccountForUser_ReturnsConflictResult_WhenAccountAlreadyExists()
    {
        // Arrange
        var userId = "user123";
        var createUserRequest = new CreateAccountRequest { Balance = 150.0M };
        var account = new Account { Id = "existingAccount", UserId = userId, Balance = 100.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Users.ContainsKey(userId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Set(account.Id, account)).Returns(false); // Account already exists
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.CreateAccountForUser(userId, createUserRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
        var conflictResult = result as ConflictObjectResult;
        Assert.Equal("An account with the same ID already exists.", (conflictResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void DeleteAccount_ReturnsNoContent_WhenAccountIsDeleted()
    {
        // Arrange
        var accountId = "account123";
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.Remove(accountId)).Returns(true);
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.DeleteAccount(accountId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteAccount_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = "nonexistentAccount";
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.Remove(accountId)).Returns(false);
        var controller = new AccountsController(dataStoreMock.Object);

        // Act
        var result = controller.DeleteAccount(accountId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("Account not found.", (notFoundResult.Value as ErrorResponse)?.Message);
    }
}