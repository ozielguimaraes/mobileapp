﻿using System;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;

namespace Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts
{
    public sealed class ReportsCalendarThisYearQuickSelectShortcut
        : ReportsCalendarBaseQuickSelectShortcut
    {
        public ReportsCalendarThisYearQuickSelectShortcut(ITimeService timeService)
            : base(timeService, Resources.ThisYear, ReportPeriod.ThisYear)
        {
        }

        public override ReportsDateRangeParameter GetDateRange()
        {
            var thisYear = TimeService.CurrentDateTime.Year;
            var start = new DateTimeOffset(thisYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var end = start.AddYears(1).AddDays(-1);
            return ReportsDateRangeParameter
                .WithDates(start, end)
                .WithSource(ReportsSource.ShortcutThisYear);
        }
    }
}
