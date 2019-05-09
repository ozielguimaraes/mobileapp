using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [MvvmCross.Preserve(AllMembers = true)]

    public sealed class SiriShortcutsSelectReportPeriodViewModel : ViewModel
    {
        private readonly INavigationService navigationService;
        private readonly ISchedulerProvider schedulerProvider;

        public readonly BehaviorRelay<ReportPeriod> SelectReportPeriod = new BehaviorRelay<ReportPeriod>(ReportPeriod.Today);
        public IObservable<IEnumerable<SelectableReportPeriodViewModel>> ReportPeriods { get; private set; }
        public UIAction Close { get; }

        public SiriShortcutsSelectReportPeriodViewModel(
            INavigationService navigationService,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.navigationService = navigationService;
            this.schedulerProvider = schedulerProvider;

            Close = rxActionFactory.FromAsync(close);

            var reportPeriods = Enum.GetValues(typeof(ReportPeriod))
                .Cast<ReportPeriod>()
                .Where(p => p != ReportPeriod.Unknown)
                .ToImmutableList();

            ReportPeriods = SelectReportPeriod
                .Select(selectedPeriod => reportPeriods.Select(p => new SelectableReportPeriodViewModel(p, p == selectedPeriod)))
                .AsDriver(new SelectableReportPeriodViewModel[0], schedulerProvider);

            Console.WriteLine("ASD");
        }

        private Task close() => navigationService.Close(this);
    }
}
