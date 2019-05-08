using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.iOS.Presentation.Transition;
using Toggl.iOS.ViewControllers;
using Toggl.iOS.ViewControllers.Settings;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public class ModalCardPresenter : IosPresenter
    {
        private readonly FromBottomTransitionDelegate fromBottomTransitionDelegate = new FromBottomTransitionDelegate();

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(EditDurationViewModel),
            typeof(EditProjectViewModel),
            typeof(EditTimeEntryViewModel),
            typeof(SelectBeginningOfWeekViewModel),
            typeof(SelectClientViewModel),
            typeof(SelectCountryViewModel),
            typeof(SelectDateFormatViewModel),
            typeof(SelectDurationFormatViewModel),
            typeof(SelectProjectViewModel),
            typeof(SelectTagsViewModel),
            typeof(SelectWorkspaceViewModel),
            typeof(SendFeedbackViewModel),
            typeof(StartTimeEntryViewModel),
            typeof(UpcomingEventsNotificationSettingsViewModel),
        };

        public ModalCardPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            UIViewController viewController = null;

            switch (viewModel)
            {
                case EditDurationViewModel editDurationViewModel:
                    var editDurationViewController = new EditDurationViewController();
                    editDurationViewController.ViewModel = editDurationViewModel;
                    viewController = editDurationViewController;
                    break;
                case EditProjectViewModel editProjectViewModel:
                    var editProjectViewController = new EditProjectViewController();
                    editProjectViewController.ViewModel = editProjectViewModel;
                    viewController = editProjectViewController;
                    break;
                case EditTimeEntryViewModel editTimeEntryViewModel:
                    var editTimeEntryViewController = new EditTimeEntryViewController();
                    editTimeEntryViewController.ViewModel = editTimeEntryViewModel;
                    viewController = editTimeEntryViewController;
                    break;
                case SelectBeginningOfWeekViewModel selectBeginningOfWeekViewModel:
                    var selectBeginningOfWeekViewController = new SelectBeginningOfWeekViewController();
                    selectBeginningOfWeekViewController.ViewModel = selectBeginningOfWeekViewModel;
                    viewController = selectBeginningOfWeekViewController;
                    break;
                case SelectClientViewModel selectClientViewModel:
                    var selectClientViewController = new SelectClientViewController();
                    selectClientViewController.ViewModel = selectClientViewModel;
                    viewController = selectClientViewController;
                    break;
                case SelectCountryViewModel selectCountryViewModel:
                    var selectCountryViewController = new SelectCountryViewController();
                    selectCountryViewController.ViewModel = selectCountryViewModel;
                    viewController = selectCountryViewController;
                    break;
                case SelectDateFormatViewModel selectDateFormatViewModel:
                    var selectDateFormatViewController = new SelectDateFormatViewController();
                    selectDateFormatViewController.ViewModel = selectDateFormatViewModel;
                    viewController = selectDateFormatViewController;
                    break;
                case SelectDurationFormatViewModel selectDurationFormatViewModel:
                    var selectDurationViewController = new SelectDurationFormatViewController();
                    selectDurationViewController.ViewModel = selectDurationFormatViewModel;
                    viewController = selectDurationViewController;
                    break;
                case SelectProjectViewModel selectProjectViewModel:
                    var selectProjectViewController = new SelectProjectViewController();
                    selectProjectViewController.ViewModel = selectProjectViewModel;
                    viewController = selectProjectViewController;
                    break;
                case SelectTagsViewModel selectTagsViewModel:
                    var selectTagsViewController = new SelectTagsViewController();
                    selectTagsViewController.ViewModel = selectTagsViewModel;
                    viewController = selectTagsViewController;
                    break;
                case SelectWorkspaceViewModel selectWorkspaceViewModel:
                    var selectWorkspaceViewController = new SelectWorkspaceViewController();
                    selectWorkspaceViewController.ViewModel = selectWorkspaceViewModel;
                    viewController = selectWorkspaceViewController;
                    break;
                case SendFeedbackViewModel sendFeedbackViewModel:
                    var sendFeedbackViewController = new SendFeedbackViewController();
                    sendFeedbackViewController.ViewModel = sendFeedbackViewModel;
                    viewController = sendFeedbackViewController;
                    break;
                case StartTimeEntryViewModel startTimeEntryViewModel:
                    var startTimeEntryViewController = new StartTimeEntryViewController();
                    startTimeEntryViewController.ViewModel = startTimeEntryViewModel;
                    viewController = startTimeEntryViewController;
                    break;
                case UpcomingEventsNotificationSettingsViewModel upcomingEventsNotificationSettingsViewModel:
                    var upcomingEventsNotificationSettingsViewController = new UpcomingEventsNotificationSettingsViewController();
                    upcomingEventsNotificationSettingsViewController.ViewModel = upcomingEventsNotificationSettingsViewModel;
                    viewController = upcomingEventsNotificationSettingsViewController;
                    break;
            }

            if (viewController == null)
                throw new Exception($"Failed to create ViewController for ViewModel of type {viewModel.GetType().Name}");

            presentViewController(viewController);
        }

        private void presentViewController(UIViewController viewController)
        {
            viewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            viewController.TransitioningDelegate = fromBottomTransitionDelegate;

            UIViewController topmostViewController = FindPresentedViewController();
            topmostViewController.PresentViewController(viewController, true, null);
        }
    }
}
