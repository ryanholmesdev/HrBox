using HrBoxApi.Data;
using HrBoxApi.Models;
using HrBoxApi.Models.DB;
using HrBoxApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HrBoxApi.Services
{
  public class TokenService : ITokenService
  {
    private readonly AppDbContext _context;
    private readonly AppSettings _appSettings;

    public TokenService(AppDbContext context, IOptions<AppSettings> appSettings)
    {
      _context = context;
      _appSettings = appSettings.Value;
    }

    public TokenResponse GenerateToken(User user)
    {
      DateTime expiryDate = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiryMinutes);
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
          {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim("EmailVerified", user.EmailVerified.ToString())
          }),
        Expires = expiryDate,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
      string token = tokenHandler.WriteToken(securityToken);
      string refreshToken = GenerateRandomString();

      SaveUserToken(token, refreshToken, user.Id, expiryDate);

      return new TokenResponse(token, refreshToken);
    }

    public TokenResponse RefreshToken(string token, string refreshToken)
    {
      int? userid = ValidateTokenAndGetUserID(token);

      if (userid != null)
      {
        UserToken userToken = _context.UserTokens.SingleOrDefault(t => t.Token == token && t.UserID == userid);

        if (userToken != null && userToken.RefreshToken == refreshToken && userToken.Token == token && userToken.RefreshTokenExpiryDateUtc > DateTime.UtcNow)
        {
          User user = _context.Users.Single(u => u.Id == userid);

          TokenResponse response = GenerateToken(user);

          return response;
        }
        else
        {
          // Unable to find the user's refresh token because it's been deleted on TokenJob or its expired.
          return null;
        }
      }
      else
      {
        return null;
      }
    }

    private void SaveUserToken(string token, string refreshToken, int userID, DateTime expiryDate)
    {
      UserToken userToken = new UserToken()
      {
        Id = 0,
        Token = token,
        RefreshToken = refreshToken,
        UserID = userID,
        TokenExpiryDateUtc = expiryDate,
        RefreshTokenExpiryDateUtc = DateTime.UtcNow.AddMinutes(_appSettings.RefreshExpiryMinutes)
      };

      // Delete any old tokens for that user.
      _context.UserTokens.RemoveRange(_context.UserTokens.Where(t => t.UserID == userID));

      _context.UserTokens.Add(userToken);

      _context.SaveChanges();
    }

    private int? ValidateTokenAndGetUserID(string token)
    {
      try
      {
        TokenValidationParameters tokenValidationParams = new TokenValidationParameters
        {
          ValidateAudience = false, // You might need to validate this one depending on your case
          ValidateIssuer = false,
          ValidateActor = false,
          ValidateLifetime = false, // Do not validate lifetime here
          IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_appSettings.JWTSecret)
            )
        };

        SecurityToken securityToken;
        ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParams, out securityToken);

        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
          throw new SecurityTokenException("Invalid token!");
        }

        string userId = principal.FindFirstValue(ClaimTypes.Name);
        if (userId != null && string.IsNullOrEmpty(userId))
        {
          throw new SecurityTokenException($"Missing claim: {ClaimTypes.Name}!");
        }

        return Convert.ToInt32(userId);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    private string GenerateRandomString()
    {
      //TODO: Put a random guid on top of it too.
      byte[] randomNumber = new byte[32];
      using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }
  }
}
