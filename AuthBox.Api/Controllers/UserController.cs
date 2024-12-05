using AuthBox.Api.Repositories.Interfaces;
using AuthBox.Models.Dtos;
using AuthBox.Models.Dtos.Users;
using AuthBox.Models.Enums;
using AuthBox.Utils.ExtensionMethods;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AuthBox.Api.Controllers;

public class UserController : BaseController
{
    private readonly IUserRepository _repo;

    public UserController(IUserRepository repo)
    {
        _repo = repo;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Register([FromBody] RegisterRequestDto registerInfo)
    {
        RegisterRequestValidator validator = new();
        ValidationResult validationResult = validator.Validate(registerInfo);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ResponseDto()
            {
                Status = EResponseStatus.ValidationFailed,
                Object = validationResult
            });
        }

        string? repositoryMessage = _repo.Register(registerInfo);

        if (repositoryMessage.IsNotNullOrEmpty())
        {
            return BadRequest(new ResponseDto()
            {
                Status = EResponseStatus.RepositoryFailed,
                Object = repositoryMessage
            });
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginInfo)
    {
        LoginRequestValidator validator = new();
        ValidationResult validationResult = validator.Validate(loginInfo);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ResponseDto()
            {
                Status = EResponseStatus.ValidationFailed,
                Object = validationResult
            });
        }

        (TokenDto? tokenDto, string? repositoryMessage) = await _repo.Login(loginInfo);

        if (repositoryMessage.IsNotNullOrEmpty())
        {
            return Unauthorized(new ResponseDto()
            {
                Status = EResponseStatus.RepositoryFailed,
                Object = repositoryMessage,
            });
        }

        Debug.Assert(tokenDto is not null);

        return Ok(tokenDto);
    }
}
