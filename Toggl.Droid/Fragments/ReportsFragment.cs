﻿using System;
using System.Reactive.Linq;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment : ReactiveFragment<ReportsViewModel>, IScrollableToTop
    {
        private static readonly TimeSpan toggleCalendarThrottleDuration = TimeSpan.FromMilliseconds(300);
        private ReportsRecyclerAdapter reportsRecyclerAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ReportsFragment, container, false);
            InitializeViews(view);
            setupToolbar();

            selectWorkspaceFAB.Rx().Tap()
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            calendarView.SetupWith(ViewModel.CalendarViewModel);

            setupReportsRecyclerView();
            ViewModel.StartDate.CombineLatest(
                    ViewModel.EndDate,
                    ViewModel.WorkspaceHasBillableFeatureEnabled,
                    ViewModel.BarChartViewModel.DateFormat,
                    ViewModel.BarChartViewModel.Bars,
                    ViewModel.BarChartViewModel.MaximumHoursPerBar,
                    ViewModel.BarChartViewModel.HorizontalLegend,
                    BarChartData.Create)
                .Subscribe(reportsRecyclerAdapter.UpdateBarChart)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceNameObservable
                .Subscribe(reportsRecyclerAdapter.UpdateWorkspaceName)
                .DisposedBy(DisposeBag);

            ViewModel.SegmentsObservable.CombineLatest(
                    ViewModel.ShowEmptyStateObservable,
                    ViewModel.TotalTimeObservable,
                    ViewModel.TotalTimeIsZeroObservable,
                    ViewModel.BillablePercentageObservable,
                    ViewModel.DurationFormatObservable,
                    ReportsSummaryData.Create)
                .Subscribe(reportsRecyclerAdapter.UpdateReportsSummary)
                .DisposedBy(DisposeBag);

            reportsRecyclerAdapter.SummaryCardClicks
                .Subscribe(hideCalendar)
                .DisposedBy(DisposeBag);

            toolbarCurrentDateRangeText.Rx().Tap()
                .Throttle(toggleCalendarThrottleDuration)
                .Subscribe(toggleCalendar)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentDateRangeStringObservable
                .Subscribe(toolbarCurrentDateRangeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            ViewModel.CalendarViewModel.ViewAppeared();
        }

        public void ScrollToTop()
        {
            reportsRecyclerView.SmoothScrollToPosition(0);
        }

        private void setupReportsRecyclerView()
        {
            reportsRecyclerAdapter = new ReportsRecyclerAdapter(Context);
            reportsRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            reportsRecyclerView.SetAdapter(reportsRecyclerAdapter);
        }

        private void setupToolbar()
        {
            var activity = Activity as AppCompatActivity;
            toolbar.Title = "";
            activity.SetSupportActionBar(toolbar);
        }

        private void toggleCalendar()
        {
            reportsMainContainer.ToggleCalendar(false);
            ViewModel.CalendarViewModel.SelectStartOfSelectionIfNeeded();
        }

        private void hideCalendar()
        {
            reportsMainContainer.ToggleCalendar(true);
            ViewModel.CalendarViewModel.SelectStartOfSelectionIfNeeded();
        }
    }
}
