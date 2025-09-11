using ONT_PROJECT.Models;
using System;

namespace ONT_PROJECT.Helpers
{
    public static class ActivityLogger
    {
        public static void LogActivity(ApplicationDbContext context, string activityType, string description)
        {
            var log = new ActivityLog
            {
                ActivityType = activityType,
                Description = description,
                DatePerformed = DateTime.Now
            };

            context.ActivityLogs.Add(log);
            context.SaveChanges();
        }
    }
}
