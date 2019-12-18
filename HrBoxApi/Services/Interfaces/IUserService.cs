using HrBoxApi.Models;
using HrBoxApi.Models.DB;
using System.Threading.Tasks;

namespace HrBoxApi.Services.Interfaces
{
  public interface IUserService
  {
    Task<Response> CreateUserAsync(User user);
    Response VerifyUser(string email, string verifyCode);
  }
}
