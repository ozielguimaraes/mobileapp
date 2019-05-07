using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using MvvmCross.ViewModels;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Diagnostics;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.Services;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class MainTabBarViewModel : ViewModel
    {
        private readonly IRemoteConfigService remoteConfigService;
        private readonly IStopwatchProvider stopwatchProvider;
        private readonly IPlatformInfo platformInfo;

        private readonly MainViewModel mainViewModel;
        private readonly ReportsViewModel reportsViewModel;
        private readonly CalendarViewModel calendarViewModel;
        private readonly SettingsViewModel settingsViewModel;

        private bool hasOpenedReports = false;

        public IList<MvxViewModel> Tabs { get; }

        public MainTabBarViewModel(
            ITimeService timeService,
            ITogglDataSource dataSource,
            ISyncManager syncManager,
            IDialogService dialogService,
            IRatingService ratingService,
            IUserPreferences userPreferences,
            IAnalyticsService analyticsService,
            IBackgroundService backgroundService,
            IInteractorFactory interactorFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            IPermissionsService permissionsService,
            INavigationService navigationService,
            IRemoteConfigService remoteConfigService,
            ISuggestionProviderContainer suggestionProviders,
            IIntentDonationService intentDonationService,
            IAccessRestrictionStorage accessRestrictionStorage,
            IStopwatchProvider stopwatchProvider,
            IRxActionFactory rxActionFactory,
            IUserAccessManager userAccessManager,
            IPrivateSharedStorageService privateSharedStorageService,
            IPlatformInfo platformInfo)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(ratingService, nameof(ratingService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(permissionsService, nameof(permissionsService));
            Ensure.Argument.IsNotNull(remoteConfigService, nameof(remoteConfigService));
            Ensure.Argument.IsNotNull(suggestionProviders, nameof(suggestionProviders));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));
            Ensure.Argument.IsNotNull(intentDonationService, nameof(intentDonationService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(stopwatchProvider, nameof(stopwatchProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(privateSharedStorageService, nameof(privateSharedStorageService));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));

            this.remoteConfigService = remoteConfigService;
            this.stopwatchProvider = stopwatchProvider;
            this.platformInfo = platformInfo;

            mainViewModel = new MainViewModel(
                dataSource,
                syncManager,
                timeService,
                ratingService,
                userPreferences,
                analyticsService,
                onboardingStorage,
                interactorFactory,
                navigationService,
                remoteConfigService,
                suggestionProviders,
                intentDonationService,
                accessRestrictionStorage,
                schedulerProvider,
                stopwatchProvider,
                rxActionFactory);

            reportsViewModel = new ReportsViewModel(
                dataSource,
                timeService,
                navigationService,
                interactorFactory,
                analyticsService,
                dialogService,
                intentDonationService,
                schedulerProvider,
                stopwatchProvider,
                rxActionFactory);

            calendarViewModel = new CalendarViewModel(
                dataSource,
                timeService,
                dialogService,
                userPreferences,
                analyticsService,
                backgroundService,
                interactorFactory,
                onboardingStorage,
                schedulerProvider,
                permissionsService,
                navigationService,
                stopwatchProvider,
                rxActionFactory);

            settingsViewModel = new SettingsViewModel(
                dataSource,
                syncManager,
                platformInfo,
                dialogService,
                userPreferences,
                analyticsService,
                userAccessManager,
                interactorFactory,
                onboardingStorage,
                navigationService,
                privateSharedStorageService,
                intentDonationService,
                stopwatchProvider,
                rxActionFactory,
                permissionsService,
                schedulerProvider);

            Tabs = getViewModels().ToList();
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await Tabs
                .Select(vm => vm.Initialize())
                .Apply(Task.WhenAll);
        }

        public void StartReportsStopwatch()
        {
            if (!hasOpenedReports)
            {
                var reportsStopwatch = stopwatchProvider.CreateAndStore(MeasuredOperation.OpenReportsViewForTheFirstTime);
                reportsStopwatch.Start();
                hasOpenedReports = true;
            }
        }

        private IEnumerable<MvxViewModel> getViewModels()
        {
            yield return mainViewModel;
            yield return reportsViewModel;
            yield return calendarViewModel;

            if (platformInfo.Platform == Platform.Giskard)
            {
                yield return settingsViewModel;
            }
        }
    }
}
