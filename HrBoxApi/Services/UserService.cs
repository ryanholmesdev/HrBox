using HrBoxApi.Data;
using HrBoxApi.Models;
using HrBoxApi.Models.DB;
using HrBoxApi.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrBoxApi.Services
{
  public class UserService : IUserService
  {
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
      _context = context;
    }

    public async Task<Response> CreateUserAsync(User user)
    {
      // Check that the users email doesn't already exist
      bool emailExists =  _context.Users.Any(u => u.Email == user.Email);
      if (!emailExists)
      {       
        user.Password = HashPassword(user.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new Response(true);
      }
      else
      {
        return new Response(false, "The email address is already in use");
      }
    }

    private string HashPassword(string password)
    {
      int hashLevel = 14;
      return BCrypt.Net.BCrypt.HashPassword(password, hashLevel);
    }

    //public static IEnumerable<User> GetUsersWithoutPassword()
    //{
    //  var users = _context.Users.Select(u => RemovePasswordDisplay(u));
    //}

    private static User RemovePasswordDisplay(User user)
    {
      user.Password = null;
      return user;
    }
  }
}
