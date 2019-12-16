using HrBoxApi.Data;
using HrBoxApi.Models;
using HrBoxApi.Models.DB;
using HrBoxApi.Services.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
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
      bool emailExists = _context.Users.Any(u => u.Email == user.Email);
      if (!emailExists)
      {
        bool isDisposableEmail = IsEmailDomainBlocked(user.Email);

        if (!isDisposableEmail)
        {
          user.Password = HashPassword(user.Password);

          _context.Users.Add(user);
          await _context.SaveChangesAsync();
        }
        else
        {
          return new Response(false, "The email address is a known disposable email.");
        }
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

    private static bool IsEmailDomainBlocked(string email)
    {
      List<string> blockedEmailDomains = new List<string>();

      Assembly thisAssembly = Assembly.GetExecutingAssembly();
      using (Stream stream = thisAssembly.GetManifestResourceStream("HrBoxApi.Resources.disposable-email-domains.json"))
      {
        using (StreamReader reader = new StreamReader(stream))
        {
          string fileJson = reader.ReadToEnd();
          blockedEmailDomains = JsonConvert.DeserializeObject<List<string>>(fileJson);
        }
      }

      MailAddress address = new MailAddress(email);
      string host = address.Host;

      return blockedEmailDomains.Contains(host);
    }
  }
}
