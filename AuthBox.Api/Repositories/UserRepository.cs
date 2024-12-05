using AuthBox.Api.Repositories.Interfaces;
using AuthBox.Models.Dtos;
using AuthBox.Models.Enums;
using AuthBox.Models.Models;
using AuthBox.Utils.ExtensionMethods;
using AuthBox.Utils.Objects;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace AuthBox.Api.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(DatabaseContext db) : base(db)
    {

    }

    public string? Register(RegisterUserDto registerInfo)
    {
        try
        {
            HashedPassword hashedPassword = HashPassword(registerInfo.Password);

            registerInfo.Password = "";
            registerInfo.PasswordConfirmation = "";

            _db.Add(new User()
            {
                Id = 0,
                Email = registerInfo.Email,
                PasswordHash = hashedPassword.Hash,
                PasswordSalt = hashedPassword.Salt,
                Role = EUserRoles.User // NOTE(serafa.leo): Default user role, the only one that can be created from this endpoint
            });

            return null;
        }
        catch (Exception ex)
        {
            string message = ex.GetInnermostMessage();

            if (message.Contains($"IX_{nameof(_db.Users)}_{nameof(User.Email)}_{nameof(User.Role)}"))
            {
                return "Já existe um usuário com esse Email cadastrado.";
            }

            throw;
        }
    }

    private static HashedPassword HashPassword(string plainPassword, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(128 / 8);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(plainPassword, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        return new() { Hash = hash, Salt = salt };
    }
}
