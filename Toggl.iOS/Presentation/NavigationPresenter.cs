using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.iOS.ViewControllers;
using Toggl.iOS.ViewControllers.Settings;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public class NavigationPresenter : IosPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(BrowserViewModel),
            typeof(CalendarSettingsViewModel),
            typeof(ForgotPasswordViewModel),
            typeof(NotificationSettingsViewModel),
            typeof(SettingsViewModel),
            typeof(SyncFailuresViewModel),
        };

        public NavigationPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView view)
        {
            UIViewController viewController = null;

            switch (viewModel)
            {
                case BrowserViewModel browserViewModel:
                    var browserViewController = new BrowserViewController();
                    browserViewController.ViewModel = browserViewModel;
                    viewController = browserViewController;
                    break;
                case CalendarSettingsViewModel calendarSettingsViewModel:
                    var calendarSettingsViewController = new CalendarSettingsViewController();
                    calendarSettingsViewController.ViewModel = calendarSettingsViewModel;
                    viewController = calendarSettingsViewController;
                    break;
                case ForgotPasswordViewModel forgotPasswordViewModel:
                    var forgotPasswordViewController = new ForgotPasswordViewController();
                    forgotPasswordViewController.ViewModel = forgotPasswordViewModel;
                    viewController = forgotPasswordViewController;
                    break;
                case NotificationSettingsViewModel notificationSettingsViewModel:
                    var notificationSettingsViewController = new NotificationSettingsViewController();
                    notificationSettingsViewController.ViewModel = notificationSettingsViewModel;
                    viewController = notificationSettingsViewController;
                    break;
                case SettingsViewModel settingsViewModel:
                    var settingsViewController = new SettingsViewController();
                    settingsViewController.ViewModel = settingsViewModel;
                    viewController = settingsViewController;
                    break;
                case SyncFailuresViewModel syncFailuresViewModel:
                    var syncFailuresViewController = new SyncFailuresViewController();
                    syncFailuresViewController.ViewModel = syncFailuresViewModel;
                    viewController = syncFailuresViewController;
                    break;
            }

            if (viewController == null)
                throw new Exception($"Failed to create ViewController for ViewModel of type {viewModel.GetType().Name}");

            presentChildNavigationController(viewController);
        }

        private void presentChildNavigationController(UIViewController viewController)
        {
            var presentedViewController = FindPresentedViewController();

            if (tryPushOnViewController(presentedViewController, viewController))
                return;

            if (presentedViewController is UITabBarController tabBarController)
            {
                var selectedController = tabBarController.SelectedViewController;

                if (!tryPushOnViewController(selectedController, viewController))
                    throw new Exception($"Failed to find a navigation controller to present view controller of type {viewController.GetType().Name}");
            }
        }

        private bool tryPushOnViewController(UIViewController parentViewController,
            UIViewController childViewController)
        {
            if (parentViewController is UINavigationController navigationController)
            {
                navigationController.PushViewController(childViewController, true);
                return true;
            }

            return false;
        }
    }
}
