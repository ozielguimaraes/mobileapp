using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
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
        private readonly IInteractorFactory interactorFactory;
        private readonly BehaviorSubject<long?> workspace = new BehaviorSubject<long?>(null);

        public readonly BehaviorRelay<ReportPeriod> SelectReportPeriod = new BehaviorRelay<ReportPeriod>(ReportPeriod.Today);
        public IObservable<IEnumerable<SelectableReportPeriodViewModel>> ReportPeriods { get; private set; }
        public UIAction Close { get; }
        public UIAction PickWorkspace { get; }

        public IObservable<string> WorkspaceName { get; }

        public SiriShortcutsSelectReportPeriodViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.navigationService = navigationService;
            this.schedulerProvider = schedulerProvider;
            this.interactorFactory = interactorFactory;

            Close = rxActionFactory.FromAsync(close);
            PickWorkspace = rxActionFactory.FromAsync(pickWorkspace);

            var reportPeriods = Enum.GetValues(typeof(ReportPeriod))
                .Cast<ReportPeriod>()
                .Where(p => p != ReportPeriod.Unknown)
                .ToImmutableList();

            ReportPeriods = SelectReportPeriod
                .Select(selectedPeriod => reportPeriods.Select(p => new SelectableReportPeriodViewModel(p, p == selectedPeriod)))
                .AsDriver(new SelectableReportPeriodViewModel[0], schedulerProvider);

            WorkspaceName = workspace
                .Where(id => id != null)
                .SelectMany(id => interactorFactory.GetWorkspaceById((long)id).Execute())
                .Select(workspace => workspace.Name)
                .AsDriver(schedulerProvider);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
            workspace.OnNext(defaultWorkspace.Id);
        }
        private Task close() => navigationService.Close(this);

        private async Task pickWorkspace()
        {

            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>(
                    "SiriShortcutsSelectReportPeriodViewModel.PickWorkspace")
                .Execute();

            var selectWorkspaceParams = new SelectWorkspaceParameters(Resources.SelectWorkspace, workspace.Value ?? defaultWorkspace.Id);
            var selectedWorkspaceId =
                await navigationService
                    .Navigate<SelectWorkspaceViewModel, SelectWorkspaceParameters, long>(selectWorkspaceParams);

            workspace.OnNext(selectedWorkspaceId);
        }
    }
}
