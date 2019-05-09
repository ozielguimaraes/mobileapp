using System;
using System.Collections.Generic;
using Android.Support.V7.App;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.Droid.Fragments;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Toggl.Droid.Presentation
{
    public class DialogFragmentPresenter : AndroidPresenter
    {
        protected override HashSet<Type> AcceptedViewModels { get; } = new HashSet<Type>
        {
            typeof(CalendarPermissionDeniedViewModel),
            typeof(NoWorkspaceViewModel),
            typeof(SelectBeginningOfWeekViewModel),
            typeof(SelectColorViewModel),
            typeof(SelectDateFormatViewModel),
            typeof(SelectDefaultWorkspaceViewModel),
            typeof(SelectDurationFormatViewModel),
            typeof(SelectUserCalendarsViewModel),
            typeof(TermsOfServiceViewModel),
            typeof(UpcomingEventsNotificationSettingsViewModel)
        };

        protected override void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var fragmentManager = tryGetFragmentManager(sourceView);
            if (fragmentManager == null)
                throw new Exception($"Parent ViewModel's view trying to present {viewModel.GetType().Name} doesn't have a FragmentManager");

            var dialog = createAndPrepareReactiveDialog(viewModel);
            dialog.Show(fragmentManager, dialog.GetType().Name);
        }

        private DialogFragment createAndPrepareReactiveDialog<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
        {
            switch (viewModel)
            {
                case CalendarPermissionDeniedViewModel calendarPermissionDeniedViewModel:
                    var calendarPermissionDeniedDialog = new CalendarPermissionDeniedFragment();
                    calendarPermissionDeniedDialog.ViewModel = calendarPermissionDeniedViewModel;
                    return calendarPermissionDeniedDialog;

                case NoWorkspaceViewModel noWorkspaceViewModel:
                    var noWorkspaceDialog = new NoWorkspaceFragment();
                    noWorkspaceDialog.ViewModel = noWorkspaceViewModel;
                    return noWorkspaceDialog;

                case SelectBeginningOfWeekViewModel selectBeginningOfWeekViewModel:
                    var selectBeginningOfWeekDialog = new SelectBeginningOfWeekFragment();
                    selectBeginningOfWeekDialog.ViewModel = selectBeginningOfWeekViewModel;
                    return selectBeginningOfWeekDialog;

                case SelectColorViewModel selectColorViewModel:
                    var selectColorDialog = new SelectColorFragment();
                    selectColorDialog.ViewModel = selectColorViewModel;
                    return selectColorDialog;

                case SelectDateFormatViewModel selectDateFormatViewModel:
                    var selectDateFormatDialog = new SelectDateFormatFragment();
                    selectDateFormatDialog.ViewModel = selectDateFormatViewModel;
                    return selectDateFormatDialog;

                case SelectDefaultWorkspaceViewModel selectDefaultWorkspaceViewModel:
                    var selectDefaultWorkspaceDialog = new SelectDefaultWorkspaceFragment();
                    selectDefaultWorkspaceDialog.ViewModel = selectDefaultWorkspaceViewModel;
                    return selectDefaultWorkspaceDialog;

                case SelectDurationFormatViewModel selectDurationFormatViewModel:
                    var selectDurationFormatDialog = new SelectDurationFormatFragment();
                    selectDurationFormatDialog.ViewModel = selectDurationFormatViewModel;
                    return selectDurationFormatDialog;

                case SelectUserCalendarsViewModel selectUserCalendarsViewModel:
                    var selectUserCalendarsDialog = new SelectUserCalendarsFragment();
                    selectUserCalendarsDialog.ViewModel = selectUserCalendarsViewModel;
                    return selectUserCalendarsDialog;

                case TermsOfServiceViewModel termsOfServiceViewModel:
                    var termsOfServiceDialog = new TermsOfServiceFragment();
                    termsOfServiceDialog.ViewModel = termsOfServiceViewModel;
                    return termsOfServiceDialog;

                case UpcomingEventsNotificationSettingsViewModel upcomingEventsNotificationSettingsViewModel:
                    var upcomingEventsNotificationSettingsDialog = new UpcomingEventsNotificationSettingsFragment();
                    upcomingEventsNotificationSettingsDialog.ViewModel = upcomingEventsNotificationSettingsViewModel;
                    return upcomingEventsNotificationSettingsDialog;
            }
            
            throw new InvalidOperationException($"There's no reactive dialog implementation for {viewModel.GetType().Name}");
        }

        private FragmentManager tryGetFragmentManager(IView sourceView)
        {
            if (sourceView is AppCompatActivity activity)
                return activity.SupportFragmentManager;

            if (sourceView is Fragment fragment)
                return fragment.FragmentManager;

            return null;
        }
    }
}