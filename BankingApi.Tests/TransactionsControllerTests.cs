using BankingApi.Controllers;
using BankingApi.Data;
using BankingApi.Entities;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace BankingApi.Tests;

public class TransactionsControllerTests
{
    [Fact]
    public void GetTransactionsForAccount_ReturnsOkResult_WithTransactions()
    {
        // Arrange
        var accountId = "account123";
        var account = new Account { Id = accountId, UserId = "user123", Balance = 100.0M };
        var transaction1 = new Transaction { Id = "transaction1", AccountId = accountId, Type = "deposit", Amount = 50.0M };
        var transaction2 = new Transaction { Id = "transaction2", AccountId = accountId, Type = "withdrawal", Amount = 30.0M };

        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Get(accountId)).Returns(account);
        dataStoreMock.Setup(d => d.Transactions.GetAll()).Returns(new List<Transaction> { transaction1, transaction2 });
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.GetTransactionsForAccount(accountId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsType<List<Transaction>>(okResult.Value);
        var transactions = okResult.Value as List<Transaction>;
        Assert.Equal(2, transactions.Count);
    }

    [Fact]
    public void GetTransactionsForAccount_ReturnsNotFoundResult_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = "nonexistentAccount";
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(false);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.GetTransactionsForAccount(accountId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("Account not found.", (notFoundResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void Deposit_ReturnsOkResult_WhenDepositIsSuccessful()
    {
        // Arrange
        var accountId = "account123";
        var amountRequest = new AmountRequest { Amount = 50.0M };
        var account = new Account { Id = accountId, UserId = "user123", Balance = 100.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Get(accountId)).Returns(account);
        dataStoreMock.Setup(d => d.Transactions.Set(It.IsAny<string>(), It.IsAny<Transaction>())).Returns(true);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Deposit(accountId, amountRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(account, okResult.Value);
    }

    [Fact]
    public void Deposit_ReturnsBadRequestResult_WhenDepositAmountIsInvalid()
    {
        // Arrange
        var accountId = "account123";
        var amountRequest = new AmountRequest { Amount = 15000.0M }; // Invalid deposit amount
        var dataStoreMock = new Mock<IDataStore>();
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Deposit(accountId, amountRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.Equal("Cannot deposit more than $10000 in a single transaction.", (badRequestResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void Deposit_ReturnsNotFoundResult_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = "nonexistentAccount";
        var amountRequest = new AmountRequest { Amount = 50.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(false);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Deposit(accountId, amountRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("Account not found.", (notFoundResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void Withdraw_ReturnsOkResult_WhenWithdrawalIsSuccessful()
    {
        // Arrange
        var accountId = "account123";
        var amountRequest = new AmountRequest { Amount = 50.0M };
        var account = new Account { Id = accountId, UserId = "user123", Balance = 500.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Get(accountId)).Returns(account);
        dataStoreMock.Setup(d => d.Transactions.Set(It.IsAny<string>(), It.IsAny<Transaction>())).Returns(true);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Withdraw(accountId, amountRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal(account, okResult.Value);
    }

    [Fact]
    public void Withdraw_ReturnsBadRequestResult_WhenWithdrawalIsInvalid()
    {
        // Arrange
        var accountId = "account123";
        var amountRequest = new AmountRequest { Amount = 70.0M };
        var account = new Account { Id = accountId, UserId = "user123", Balance = 100.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(true);
        dataStoreMock.Setup(d => d.Accounts.Get(accountId)).Returns(account);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Withdraw(accountId, amountRequest); // less that 100 will be left

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        Assert.Equal("Withdrawal request does not meet the required criteria.", (badRequestResult.Value as ErrorResponse)?.Message);
    }

    [Fact]
    public void Withdraw_ReturnsNotFoundResult_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = "nonexistentAccount";
        var amountRequest = new AmountRequest { Amount = 50.0M };
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(d => d.Accounts.ContainsKey(accountId)).Returns(false);
        var controller = new TransactionsController(dataStoreMock.Object);

        // Act
        var result = controller.Withdraw(accountId, amountRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.Equal("Account not found.", (notFoundResult.Value as ErrorResponse)?.Message);
    }
}
