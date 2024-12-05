using AuthBox.Api.Repositories.Interfaces;
using AuthBox.Models.Dtos;
using AuthBox.Models.Enums;
using AuthBox.Utils.ExtensionMethods;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
    public IActionResult Register([FromBody] RegisterUserDto registerInfo)
    {
        RegisterUserValidator validator = new();
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
}
