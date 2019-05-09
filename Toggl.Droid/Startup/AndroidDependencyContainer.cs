using System;
using Toggl.Droid.Services;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Diagnostics;
using Toggl.Core.Login;
using Toggl.Core.UI;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Suggestions;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Realm;
using Toggl.Storage.Settings;
using Toggl.Networking;
using Toggl.Networking.Network;
using Android.Content;
using Android.App;
using Toggl.Core.UI.Navigation;
using Toggl.Droid.Presentation;

namespace Toggl.Droid
{
    public sealed class AndroidDependencyContainer : UIDependencyContainer
    {
        private const int numberOfSuggestions = 5;

        private readonly Lazy<SettingsStorage> settingsStorage;

        public ActivityPresenter ActivityPresenter { get; } = new ActivityPresenter();
        
        public new static AndroidDependencyContainer Instance { get; private set; }

        public static void EnsureInitialized(ApiEnvironment environment, Platform platform, string version)
        {
            if (Instance != null)
                return;

            Instance = new AndroidDependencyContainer(environment, platform, version);
            UIDependencyContainer.Instance = Instance;
        }

        private AndroidDependencyContainer(ApiEnvironment environment, Platform platform, string version)
            : base(environment, new UserAgent(platform.ToString(), version))
        {
            var appVersion = Version.Parse(version);
            
            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage));
        }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceAndroid();

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceAndroid();

        protected override IBrowserService CreateBrowserService()
            => new BrowserServiceAndroid();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceAndroid(PermissionsChecker);

        protected override ITogglDatabase CreateDatabase()
            => new Database();

        protected override IIntentDonationService CreateIntentDonationService()
            => new NoopIntentDonationServiceAndroid();

        protected override IKeyValueStorage CreateKeyValueStorage()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(Platform.Giskard.ToString(), FileCreationMode.Private);
            return new SharedPreferencesStorageAndroid(sharedPreferences);
        }

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderAndroid();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceAndroid();

        protected override IPasswordManagerService CreatePasswordManagerService()
            => new StubPasswordManagerService();

        protected override IPermissionsChecker CreatePermissionsChecker()
            => new PermissionsCheckerAndroid();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoAndroid();

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new NoopPrivateSharedStorageServiceAndroid();

        protected override IRatingService CreateRatingService()
            => new RatingServiceAndroid(Application.Context);

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigServiceAndroid();

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new AndroidSchedulerProvider(AnalyticsService);

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator(Application.Context);

        protected override IStopwatchProvider CreateStopwatchProvider()
            => new FirebaseStopwatchProviderAndroid();

        protected override ISuggestionProviderContainer CreateSuggestionProviderContainer()
            => new SuggestionProviderContainer(
                new MostUsedTimeEntrySuggestionProvider(Database, TimeService, numberOfSuggestions)
            );

        protected override INavigationService CreateNavigationService()
            => new NavigationService(
                new CompositePresenter(ActivityPresenter, new DialogFragmentPresenter()), 
                new ViewModelLoader(this),
                AnalyticsService
            );

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
            => settingsStorage.Value;

        protected override IOnboardingStorage CreateOnboardingStorage()
            => settingsStorage.Value;

        protected override IUserPreferences CreateUserPreferences()
            => settingsStorage.Value;

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
            => settingsStorage.Value;
    }
}
