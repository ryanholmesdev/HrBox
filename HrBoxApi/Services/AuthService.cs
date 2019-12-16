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
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _context;
    private readonly AppSettings _appSettings;

    public AuthService(AppDbContext context, IOptions<AppSettings> appSettings)
    {
      _context = context;
      _appSettings = appSettings.Value;
    }

    public LoginResponse LoginUser(string email, string password)
    {
      User user = _context.Users.SingleOrDefault(u => u.Email == email);

      if (user != null)
      {
        // user found
        bool isPasswordCorrect = ValidatePassword(password, user.Password);
        if (isPasswordCorrect)
        {

          // TODO: Store this somewhere in settings.
          string jwtSecret = _appSettings.JWTSecret;

          // TODO: Store expiry time in settings.
          DateTime expiryDate = DateTime.UtcNow.AddMinutes(1);

          // authentication successful so generate jwt token
          var tokenHandler = new JwtSecurityTokenHandler();
          var key = Encoding.ASCII.GetBytes(jwtSecret);
          var tokenDescriptor = new SecurityTokenDescriptor
          {
            Subject = new ClaimsIdentity(new Claim[]
              {
                new Claim(ClaimTypes.Name, user.Id.ToString())
              }),

            // TODO: Expire sooner !!
            Expires = expiryDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
          };

          SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
          string token = tokenHandler.WriteToken(securityToken);
          string refreshToken = GenerateRefreshToken();

          // save this token.
          SaveUserToken(token, refreshToken, user.Id, expiryDate);

          return new LoginResponse(true, null, token, refreshToken);
        }
        else
        {
          //TODO: Password is incorrect.
          return new LoginResponse(false, "Email or password is incorrect");
        }

      }
      else
      {
        // TODO: User email or password is incorrect.
        return new LoginResponse(false, "Email or password is incorrect");
      }
    }

    public TokenResponse RefreshToken(string token, string refreshToken)
    {
      int? userid = ValidateTokenAndGetUserID(token);
                          
      if (userid != null)
      {
        // TODO: Do we need all these where clauses ?????
        UserToken userToken = _context.UserTokens.SingleOrDefault(t => t.Token == token & t.RefreshToken == refreshToken & t.UserID == userid);
        if (userToken != null && userToken.RefreshTokenExpiryDateUtc > DateTime.UtcNow)
        {
          string jwtSecret = _appSettings.JWTSecret;

          // TODO: Store expiry time in settings.
          DateTime expiryDate = DateTime.UtcNow.AddMinutes(1);

          // authentication successful so generate jwt token
          var tokenHandler = new JwtSecurityTokenHandler();
          var key = Encoding.ASCII.GetBytes(jwtSecret);
          var tokenDescriptor = new SecurityTokenDescriptor
          {
            Subject = new ClaimsIdentity(new Claim[]
              {
                new Claim(ClaimTypes.Name, userid.ToString())
              }),

            // TODO: Expire sooner !!
            Expires = expiryDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
          };

          SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
          string newToken = tokenHandler.WriteToken(securityToken);
          string newRefreshToken = GenerateRefreshToken();

          // save this token.
          SaveUserToken(newToken, newRefreshToken, Convert.ToInt32(userid), expiryDate);

          return new TokenResponse(newToken, newRefreshToken);
        }
        else
        {
          // TODO: THROW the user token cannot be refreshed because one cannot be found or because the refresh token has expired.
          return null;
        }
      }
      else
      {
        // TODO: Catcherrors
        return null;
      }
    }

    /// <summary>
    /// Validates a jwt and returns the user id 
    /// </summary>
    private int? ValidateTokenAndGetUserID(string token)
    {
      try
      {
        string jwtSecret = _appSettings.JWTSecret;

        var tokenValidationParamters = new TokenValidationParameters
        {
          ValidateAudience = false, // You might need to validate this one depending on your case
          ValidateIssuer = false,
          ValidateActor = false,
          ValidateLifetime = false, // Do not validate lifetime here
          IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtSecret)
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParamters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
          throw new SecurityTokenException("Invalid token!");
        }

        var userId = principal.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
          throw new SecurityTokenException($"Missing claim: {ClaimTypes.Name}!");
        }

        return Convert.ToInt32(userId);
      }
      catch(Exception ex)
      {
        // TODO:// CHeck this. 
        return null;
      }
    } 

    private void SaveUserToken(string token, string refreshToken, int userID, DateTime expiryDate)
    {
      // TODO: Add refresh expiry

      UserToken userToken = new UserToken()
      {
        Id = 0,
        Token = token,
        RefreshToken = refreshToken,
        UserID = userID,
        TokenExpiryDateUtc = expiryDate,
        RefreshTokenExpiryDateUtc = DateTime.UtcNow.AddMinutes(30)
      };

      // Delete any old tokens
      _context.UserTokens.RemoveRange(_context.UserTokens.Where(t => t.UserID == userID));

      // Add the new token record
      _context.UserTokens.Add(userToken);

      _context.SaveChanges();
    }

    private bool ValidatePassword(string rawPassword, string hashedPassword)
    {
      return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
    }

    private string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }
  }
}
