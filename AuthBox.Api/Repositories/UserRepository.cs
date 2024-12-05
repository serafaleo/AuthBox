using AuthBox.Api.Repositories.Interfaces;
using AuthBox.Models.Dtos;
using AuthBox.Models.Dtos.Users;
using AuthBox.Models.Enums;
using AuthBox.Models.Models;
using AuthBox.Utils.ExtensionMethods;
using AuthBox.Utils.Objects;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthBox.Api.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
    private readonly IConfiguration _configuration;

    public UserRepository(DatabaseContext db, IConfiguration configuration) : base(db)
    {
        _configuration = configuration;
    }

    public string? Register(RegisterRequestDto registerInfo)
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
            });

            return null;
        }
        catch (Exception ex)
        {
            string message = ex.GetInnermostMessage();

            if (message.Contains($"IX_{nameof(_db.Users)}_{nameof(User.Email)}"))
            {
                return "Já existe um usuário com esse Email cadastrado.";
            }

            throw;
        }
    }

    public async Task<(TokenDto? tokenDto, string? repositoryMessage)> Login(LoginRequestDto loginInfo)
    {
        User? userFound = await _db.Users.Where(user => user.Email == loginInfo.Email).FirstOrDefaultAsync();

        if(userFound is null)
        {
            return (null, "Usuário não encontrado.");
        }

        HashedPassword hashedPassword = HashPassword(loginInfo.Password, userFound.PasswordSalt);
        loginInfo.Password = "";

        if(hashedPassword.Hash != userFound.PasswordHash)
        {
            return (null, "Senha inválida.");
        }

        List<Claim> claims = [new Claim(ClaimTypes.Email, userFound.Email)];

        TokenDto tokenDto = new()
        {
            AccessToken = GenerateAccessToken(claims),
            RefreshToken = GenerateRefreshToken()
        };

        DateTime refreshTokenExpiryTime = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenValidityInDays"]!));

        _db.Users.Where(user => user.Id == userFound.Id)
            .ExecuteUpdate(calls => calls.SetProperty(user => user.RefreshToken, tokenDto.RefreshToken)
                                         .SetProperty(user => user.RefreshTokenExpiryTime, refreshTokenExpiryTime));

        return (tokenDto, null);
    }

    private static HashedPassword HashPassword(string plainPassword, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(128 / 8);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(plainPassword, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        return new() { Hash = hash, Salt = salt };
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        SigningCredentials signinCredentials = new(secretKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken tokeOptions = new(
            issuer: _configuration["Jwt:ValidIssuer"],
            audience: _configuration["Jwt:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:TokenValidityInMinutes"]!)),
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
