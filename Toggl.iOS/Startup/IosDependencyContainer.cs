using System;
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
using Toggl.Core.UI.Navigation;
using Toggl.iOS.Presentation;
using Toggl.iOS.Services;

namespace Toggl.iOS
{
    public sealed class IosDependencyContainer : UIDependencyContainer
    {
        private const int numberOfSuggestions = 3;

        private readonly Lazy<SettingsStorage> settingsStorage;

        public TogglPresenter ViewPresenter { get; }
        public INavigationService MvxNavigationService { get; internal set; }
        public IIntentDonationService IntentDonationService { get; }

        public new static IosDependencyContainer Instance { get; private set; }

        public static void EnsureInitialized(TogglPresenter viewPresenter, ApiEnvironment environment, Platform platform, string version)
        {
            if (Instance != null)
                return;

            Instance = new IosDependencyContainer(viewPresenter, environment, platform, version);
            UIDependencyContainer.Instance = Instance;
        }

        public OnePasswordServiceIos OnePasswordService => PasswordManagerService as OnePasswordServiceIos;

        private IosDependencyContainer(TogglPresenter viewPresenter, ApiEnvironment environment, Platform platform, string version)
            : base(environment, new UserAgent(platform.ToString(), version))
        {
            ViewPresenter = viewPresenter;
            IntentDonationService = new IntentDonationService(AnalyticsService);

            var appVersion = Version.Parse(version);

            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage));
        }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceIos();

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceIos();

        protected override IBrowserService CreateBrowserService()
            => new BrowserServiceIos();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceIos(PermissionsService);

        protected override ITogglDatabase CreateDatabase()
            => new Database();

        protected override IDialogService CreateDialogService()
            => new DialogServiceIos(ViewPresenter);

        protected override IGoogleService CreateGoogleService()
            => new GoogleServiceIos();

        protected override IKeyValueStorage CreateKeyValueStorage()
            => new UserDefaultsStorageIos();

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderIos();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceIos(PermissionsService, TimeService);

        protected override IPasswordManagerService CreatePasswordManagerService()
            => new OnePasswordServiceIos();

        protected override IPermissionsService CreatePermissionsService()
            => new PermissionsServiceIos();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoIos();

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new PrivateSharedStorageServiceIos();

        protected override IRatingService CreateRatingService()
            => new RatingServiceIos();

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigServiceIos();

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new IOSSchedulerProvider();

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator();

        protected override IStopwatchProvider CreateStopwatchProvider()
            => new FirebaseStopwatchProviderIos();

        protected override ISuggestionProviderContainer CreateSuggestionProviderContainer()
            => new SuggestionProviderContainer(
                new MostUsedTimeEntrySuggestionProvider(Database, TimeService, numberOfSuggestions)
            );

        protected override INavigationService CreateNavigationService()
            => MvxNavigationService;

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
