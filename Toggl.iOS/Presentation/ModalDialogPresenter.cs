using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.Views;
using Toggl.iOS.Presentation.Transition;
using Toggl.iOS.ViewControllers;
using Toggl.iOS.ViewControllers.Calendar;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public class ModalDialogPresenter : IosPresenter
    {
        private readonly ModalDialogTransitionDelegate modalTransitionDelegate = new ModalDialogTransitionDelegate();

        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(CalendarPermissionDeniedViewModel),
            typeof(NoWorkspaceViewModel),
            typeof(SelectColorViewModel),
            typeof(SelectDateTimeViewModel),
            typeof(SelectDefaultWorkspaceViewModel),
            typeof(SelectUserCalendarsViewModel),
            typeof(TermsOfServiceViewModel),
        };

        public ModalDialogPresenter(UIWindow window, AppDelegate appDelegate) : base(window, appDelegate)
        {
        }

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            UIViewController viewController = null;

            switch (viewModel)
            {
                case CalendarPermissionDeniedViewModel calendarPermissionDeniedViewModel:
                    var calendarPermissionDeniedViewController = new CalendarPermissionDeniedViewController();
                    calendarPermissionDeniedViewController.ViewModel = calendarPermissionDeniedViewModel;
                    viewController = calendarPermissionDeniedViewController;
                    break;
                case NoWorkspaceViewModel noWorkspaceViewModel:
                    var noWorkspaceViewController = new NoWorkspaceViewController();
                    noWorkspaceViewController.ViewModel = noWorkspaceViewModel;
                    viewController = noWorkspaceViewController;
                    break;
                case SelectColorViewModel selectColorViewModel:
                    var selectColorViewController = new SelectColorViewController();
                    selectColorViewController.ViewModel = selectColorViewModel;
                    viewController = selectColorViewController;
                    break;
                case SelectDateTimeViewModel selectDateTimeViewModel:
                    var selectDateTimeViewController = new SelectDateTimeViewController();
                    selectDateTimeViewController.ViewModel = selectDateTimeViewModel;
                    viewController = selectDateTimeViewController;
                    break;
                case SelectDefaultWorkspaceViewModel selectDefaultWorkspaceViewModel:
                    var selectDefaultWorkspaceViewController = new SelectDefaultWorkspaceViewController();
                    selectDefaultWorkspaceViewController.ViewModel = selectDefaultWorkspaceViewModel;
                    viewController = selectDefaultWorkspaceViewController;
                    break;
                case SelectUserCalendarsViewModel selectUserCalendarsViewModel:
                    var selectUserCalendarsViewController = new SelectUserCalendarsViewController();
                    selectUserCalendarsViewController.ViewModel = selectUserCalendarsViewModel;
                    viewController = selectUserCalendarsViewController;
                    break;
                case TermsOfServiceViewModel termsOfServiceViewModel:
                    var termsOfServiceViewController = new TermsOfServiceViewController();
                    termsOfServiceViewController.ViewModel = termsOfServiceViewModel;
                    viewController = termsOfServiceViewController;
                    break;
            }

            if (viewController == null)
                throw new Exception($"Failed to create ViewController for ViewModel of type {viewModel.GetType().Name}");

            presentViewController(viewController);
        }

        private void presentViewController(UIViewController viewController)
        {
            viewController.ModalPresentationStyle = UIModalPresentationStyle.Custom;
            viewController.TransitioningDelegate = modalTransitionDelegate;

            UIViewController topmostViewController = FindPresentedViewController();
            topmostViewController.PresentViewController(viewController, true, null);
        }
    }
}
