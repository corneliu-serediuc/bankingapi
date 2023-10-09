using BankingApi.Data;
using BankingApi.Extensions;
using BankingApi.Entities;
using Microsoft.AspNetCore.Mvc;
using BankingApi.Models;
using System.Linq;
using System.Collections.Generic;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/v1")]
public class AccountsController : ControllerBase
{
    private readonly IDataStore _dataStore;

    public AccountsController(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    [HttpGet("users/{userId}/accounts")]
    [ProducesResponseType(typeof(List<Account>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult GetAccount(string userId)
    {
        if (!_dataStore.Users.ContainsKey(userId))
        {
            var response = new ErrorResponse { Message = "User not found." };
            return NotFound(response);
        }

        var accounts = _dataStore.Accounts.GetAll()
            .Where(t => t.UserId == userId)
            .ToList();

        return Ok(accounts);
    }

    [HttpPost("users/{userId}/accounts")]
    [ProducesResponseType(typeof(Account), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult CreateAccountForUser(string userId, [FromBody] CreateAccountRequest request)
    {
        if (!_dataStore.Users.ContainsKey(userId))
        {
            var response = new ErrorResponse { Message = "User not found." };
            return NotFound(response);
        }

        var account = new Account
        {
            UserId = userId,
            Balance = request.Balance,
        };

        if (!account.HasValidInitialBalance())
        {
            var response = new ErrorResponse { Message = "Initial balance must be at least $100." };
            return BadRequest(response);
        }

        if (_dataStore.Accounts.Set(account.Id, account))
        {
            return Ok(account);
        }
        else
        {
            var response = new ErrorResponse { Message = "An account with the same ID already exists." };
            return Conflict(response);
        }
    }

    [HttpDelete("accounts/{accountId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public IActionResult DeleteAccount(string accountId)
    {
        if (_dataStore.Accounts.Remove(accountId))
        {
            return NoContent();
        }
        else
        {
            var response = new ErrorResponse { Message = "Account not found." };
            return NotFound(response);
        }
    }
}

