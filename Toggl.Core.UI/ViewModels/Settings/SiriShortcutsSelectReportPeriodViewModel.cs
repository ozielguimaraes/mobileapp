using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Settings
{
    [MvvmCross.Preserve(AllMembers = true)]

    public sealed class SiriShortcutsSelectReportPeriodViewModel : ViewModel
    {
        private readonly INavigationService navigationService;

        public IImmutableList<SelectableReportPeriodViewModel> ReportPeriods;
        public UIAction Close { get; }

        public SiriShortcutsSelectReportPeriodViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;
            Close = rxActionFactory.FromAsync(close);

            ReportPeriods = Enum.GetValues(typeof(ReportPeriod))
                .Cast<ReportPeriod>()
                .Where(p => p != ReportPeriod.Unknown)
                .Select(p => new SelectableReportPeriodViewModel(p, false))
                .ToImmutableList();
        }

        private Task close() => navigationService.Close(this);
    }
}
