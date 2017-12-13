using System;
using System.Reactive.Concurrency;
using Foundation;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.Plugins;
using Toggl.Daneel.Presentation;
using Toggl.Daneel.Services;
using Toggl.Foundation;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Foundation.Suggestions;
using Toggl.Foundation.Sync;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Realm;
using Toggl.PrimeRadiant.Settings;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using UIKit;

namespace Toggl.Daneel
{
    public partial class Setup : MvxIosSetup
    {
        private IMvxNavigationService navigationService;

#if USE_PRODUCTION_API
        private const ApiEnvironment environment = ApiEnvironment.Production;
#else
        private const ApiEnvironment environment = ApiEnvironment.Staging;
#endif

        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : this(applicationDelegate, new TogglPresenter(applicationDelegate, window))
        {
        }

        private Setup(MvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter)
            : base(applicationDelegate, presenter)
        {
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.RegisterSingleton<IPasswordManagerService>(new OnePasswordService());
        }

        protected override IMvxTrace CreateDebugTrace() => new DebugTrace();

        protected override IMvxApplication CreateApp() => new App();

        protected override IMvxNavigationService InitializeNavigationService(IMvxViewModelLocatorCollection collection)
            => navigationService = base.InitializeNavigationService(collection);

        protected override void InitializeApp(IMvxPluginManager pluginManager, IMvxApplication app)
        {
            base.InitializeApp(pluginManager, app);

#if !USE_PRODUCTION_API
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                  += (sender, certificate, chain, sslPolicyErrors) => true;
#endif

            var scheduler = TaskPoolScheduler.Default;
            var database = new Database();
            var timeService = new TimeService(Scheduler.Default);
            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            var userAgent = new UserAgent("Daneel", version.ToString());

            var googleService = new GoogleService();
            var apiFactory = new ApiFactory(environment, userAgent);
            var userDefaultsStorage = new UserDefaultsStorage(timeService, Version.Parse(version.ToString()));
            var apiErrorHandlingService = new ApiErrorHandlingService(navigationService, userDefaultsStorage);
            var retryDelayLimit = TimeSpan.FromSeconds(60);
            
            Func<ITogglDataSource, ISyncManager> createSyncManager(ITogglApi api)
                => dataSource => TogglSyncManager.CreateSyncManager(database, api, dataSource, timeService, retryDelayLimit, scheduler);

            ITogglDataSource createDataSource(ITogglApi api)
                => new TogglDataSource(database, timeService, apiErrorHandlingService, createSyncManager(api));

            Mvx.RegisterSingleton<ITimeService>(timeService);
            Mvx.RegisterSingleton<IBrowserService>(new BrowserService());
            Mvx.RegisterSingleton<IOnboardingStorage>(userDefaultsStorage);
            Mvx.RegisterSingleton<IAccessRestrictionStorage>(userDefaultsStorage);
            Mvx.RegisterSingleton<IDialogService>(new DialogService((ITopViewControllerProvider)Presenter));
            Mvx.RegisterSingleton<ISuggestionProviderContainer>(
                new SuggestionProviderContainer(
                    new MostUsedTimeEntrySuggestionProvider(database, timeService)
                )
            );
            Mvx.RegisterSingleton<IApiErrorHandlingService>(apiErrorHandlingService);

            var togglApp = app as App;
            var loginManager = new LoginManager(apiFactory, database, googleService, userDefaultsStorage, createDataSource);
            togglApp.Initialize(loginManager, navigationService, userDefaultsStorage);
        }
    }
}
