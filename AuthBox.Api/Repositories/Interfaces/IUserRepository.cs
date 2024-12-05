using AuthBox.Models.Dtos;
using AuthBox.Models.Dtos.Users;

namespace AuthBox.Api.Repositories.Interfaces;

public interface IUserRepository
{
    public string? Register(RegisterRequestDto registerInfo);
    public Task<(TokenDto? tokenDto, string? repositoryMessage)> Login(LoginRequestDto loginInfo);
}
