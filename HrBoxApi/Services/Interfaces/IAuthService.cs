using HrBoxApi.Models;

namespace HrBoxApi.Services.Interfaces
{
  public interface IAuthService
  {
    LoginResponse LoginUser(string email, string password);
    void LogoutUser(int userId);
    TokenResponse RefreshToken(string token, string refreshToken);
  }
}
