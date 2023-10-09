using BankingApi.Data;
using BankingApi.Entities;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/v1")]
public class UsersController : ControllerBase
{
    private readonly IDataStore _dataStore;

    public UsersController(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }


    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult GetUser(string userId)
    {
        var user = _dataStore.Users.Get(userId);

        if (user == null)
        {
            var response = new ErrorResponse { Message = "User not found." };
            return NotFound(response);
        }

        return Ok(user);
    }

    [HttpPost("users")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 409)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
        };

        if (_dataStore.Users.Set(user.Id, user))
        {
            return Ok(user);
        }

        var response = new ErrorResponse { Message = "A user with the same ID already exists." };
        return Conflict(response);
    }
}
