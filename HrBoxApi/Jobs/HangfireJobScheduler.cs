using Hangfire;
using System;

namespace HrBoxApi.Jobs
{
  public static class HangfireJobScheduler
  {
    public static void ScheduleRecurringJobs()
    {
      // Delete Token job if already active in hangfire.
      RecurringJob.RemoveIfExists(nameof(TokenJob));

      // Add the token job to hangfire, every 1hr
      RecurringJob.AddOrUpdate<TokenJob>(nameof(TokenJob),
        job => job.CleanupUserTokens(),
        Cron.Hourly, TimeZoneInfo.Utc);
    }
  }
}
