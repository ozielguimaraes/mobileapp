using System;
using Foundation;
using Newtonsoft.Json;
using Toggl.iOS.ExtensionKit.Models;
using Toggl.Shared.Models;

namespace Toggl.iOS.ExtensionKit.Extensions
{
    public static class UserActivityExtensions
    {
        private const string entryDescriptionKey = "entryDescriptionKey";
        private const string responseTextKey = "responseTextKey";
        private const string timeEntryKey = "timeEntryKey";

        public static void SetEntryDescription(this NSUserActivity userActivity, string entryDescription)
            => userActivity.AddUserInfoEntries(new NSDictionary(entryDescriptionKey, entryDescription));

        public static NSString GetEntryDescription(this NSUserActivity userActivity)
           => (NSString)userActivity.UserInfo[entryDescriptionKey];

        public static void SetResponseText(this NSUserActivity userActivity, string responseText)
            => userActivity.AddUserInfoEntries(new NSDictionary(responseTextKey, responseText));

        public static NSString GetResponseText(this NSUserActivity userActivity)
           => (NSString)userActivity.UserInfo[responseTextKey];

        public static void SetTimeEntry(this NSUserActivity userActivity, ITimeEntry te)
        {
            var jsonString = JsonConvert.SerializeObject(te);
            userActivity.AddUserInfoEntries(new NSDictionary(timeEntryKey, new NSString(jsonString)));
        }

        public static ITimeEntry GetTimeEntry(this NSUserActivity userActivity)
        {
            var jsonString = (NSString) userActivity.UserInfo[timeEntryKey];
            var te = JsonConvert.DeserializeObject<TimeEntry>(jsonString);
            return te;
        }
    }
}
