using HrBoxApi.Data;
using HrBoxApi.Models;
using HrBoxApi.Models.DB;
using HrBoxApi.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
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
          user.EmailVerifyCode = GenerateEmailVerifyCode();

          // Generate email verify code.
          _context.Users.Add(user);
          await _context.SaveChangesAsync();

          // TODO: Send email verification code.
          return new Response(true);
        }
        else
        {
          return new Response(false, "The email address is a known disposable email.");
        }
      }
      else
      {
        return new Response(false, "The email address is already in use");
      }
    }

    public Response VerifyUser(string email, string verifyCode)
    {
      User user = _context.Users.Single(u => u.Email == email);

      if (user.EmailVerifyCode == verifyCode)
      {
        // remove no longer needed
        user.EmailVerifyCode = null;
        user.EmailVerified = true;

        _context.SaveChanges();

        return new Response(true);
      }
      return new Response(false, "Verify Code invalid");
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

    private string GenerateEmailVerifyCode()
    {
      StringBuilder sb = new StringBuilder();
      Random random = new Random();
      for (int i = 0; i < 5; i++)
      {
        sb.Append(random.Next(0, 10));
      }
      return sb.ToString();
    }
  }
}
