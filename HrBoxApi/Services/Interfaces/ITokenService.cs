using HrBoxApi.Models;
using HrBoxApi.Models.DB;

namespace HrBoxApi.Services.Interfaces
{
  public interface ITokenService
  {
    TokenResponse GenerateToken(User user);
    TokenResponse RefreshToken(string token, string refreshToken);
  }
}
