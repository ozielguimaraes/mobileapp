using Android.App;
using Android.App.Job;
using Android.Content;
using Toggl.Droid.Extensions;
using Toggl.Core.Services;

namespace Toggl.Droid.Services
{
    public sealed class BackgroundSyncServiceAndroid : BaseBackgroundSyncService
    {
        public override void EnableBackgroundSync()
        {
            // Background sync is temporary disabled due to a crash that is hard to reproduce
            DisableBackgroundSync();
            return;

            var context = Application.Context;
            var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            var periodicity = (long)MinimumBackgroundFetchInterval.TotalMilliseconds;
            var jobInfo = context.CreateBackgroundSyncJobInfo(periodicity);
            jobScheduler.Schedule(jobInfo);
        }

        public override void DisableBackgroundSync()
        {
            var context = Application.Context;
            var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            jobScheduler.Cancel(BackgroundSyncJobSchedulerService.JobId);
        }
    }
}
