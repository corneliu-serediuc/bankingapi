using BankingApi.Data;
using BankingApi.Entities;
using BankingApi.Extensions;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/v1")]
public class TransactionsController : ControllerBase
{
    private readonly IDataStore _dataStore;

    public TransactionsController(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    [HttpGet("account/{accountId}/transactions")]
    [ProducesResponseType(typeof(List<Transaction>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult GetTransactionsForAccount(string accountId)
    {
        if (!_dataStore.Accounts.ContainsKey(accountId))
        {
            var response = new ErrorResponse { Message = "Account not found." };
            return NotFound(response);
        }

        var transactions = _dataStore.Transactions.GetAll()
            .Where(t => t.AccountId == accountId)
            .ToList();

        return Ok(transactions);
    }

    [HttpPost("accounts/{accountId}/transactions/deposit")]
    [ProducesResponseType(typeof(Account), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult Deposit(string accountId, [FromBody] AmountRequest request)
    {
        if (!request.CanDeposit())
        {
            var response = new ErrorResponse { Message = "Cannot deposit more than $10000 in a single transaction." };
            return BadRequest(response);
        }

        if (!_dataStore.Accounts.ContainsKey(accountId))
        {
            var response = new ErrorResponse { Message = "Account not found." };
            return NotFound(response);
        }

        var account = _dataStore.Accounts.Get(accountId);

        var transaction = new Transaction
        {
            AccountId = accountId,
            Type = "deposit",
            Amount = request.Amount
        };

        account.Balance += request.Amount;
        account.UpdatedAt = DateTime.UtcNow;

        _dataStore.Transactions.Set(transaction.Id, transaction);

        return Ok(account);
    }

    [HttpPost("accounts/{accountId}/transactions/withdraw")]
    [ProducesResponseType(typeof(Account), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult Withdraw(string accountId, [FromBody] AmountRequest request)
    {
        if (!_dataStore.Accounts.ContainsKey(accountId))
        {
            var response = new ErrorResponse { Message = "Account not found." };
            return NotFound(response);
        }

        var account = _dataStore.Accounts.Get(accountId);

        if (!account.CanWithdraw(request.Amount))
        {
            var response = new ErrorResponse { Message = "Withdrawal request does not meet the required criteria." };
            return BadRequest(response);
        }

        var transaction = new Transaction
        {
            AccountId = accountId,
            Type = "withdrawal",
            Amount = request.Amount
        };

        account.Balance -= request.Amount;
        account.UpdatedAt = DateTime.UtcNow;

        _dataStore.Transactions.Set(transaction.Id, transaction);

        return Ok(account);
    }
}
