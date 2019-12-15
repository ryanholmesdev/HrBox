using HrBoxApi.Data;
using HrBoxApi.Jobs.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HrBoxApi.Jobs
{
  public class TokenJob : ITokenJob
  {
    private readonly ILogger<TokenJob> _logger;
    private readonly AppDbContext _context;

    public TokenJob(ILogger<TokenJob> logger, AppDbContext context)
    {
      _logger = logger;
      _context = context;
    }

    public async Task CleanupUserTokens()
    {
      _logger.LogInformation("Start of clean up user tokens hangfire job.");
      _context.UserTokens.RemoveRange(_context.UserTokens.Where(u => u.RefreshTokenExpiryDateUtc < DateTime.UtcNow));
      await _context.SaveChangesAsync().ConfigureAwait(true);
      _logger.LogInformation("Clean up of user tokens hangfire job completed.");
    }
  }
}
