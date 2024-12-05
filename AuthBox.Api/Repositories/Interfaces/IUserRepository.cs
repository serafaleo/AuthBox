using AuthBox.Models.Dtos;

namespace AuthBox.Api.Repositories.Interfaces;

public interface IUserRepository
{
    public string? Register(RegisterUserDto registerInfo);
}
