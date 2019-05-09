using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectWorkspaceViewModel : ViewModel<SelectWorkspaceParameters, long>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;

        private long currentWorkspaceId;

        public string Title { get; private set; }
        public ReadOnlyCollection<SelectableWorkspaceViewModel> Workspaces { get; private set; }

        public UIAction Close { get; }
        public InputAction<SelectableWorkspaceViewModel> SelectWorkspace { get; }

        public SelectWorkspaceViewModel(
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;

            Close = rxActionFactory.FromAsync(close);
            SelectWorkspace = rxActionFactory.FromAsync<SelectableWorkspaceViewModel>(selectWorkspace);
        }

        public override void Prepare(SelectWorkspaceParameters parameter)
        {
            Title = parameter.Title;
            currentWorkspaceId = parameter.CurrentWorkspaceId;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var workspaces = await interactorFactory.GetAllWorkspaces().Execute();

            Workspaces = workspaces
                .Where(w => w.IsEligibleForProjectCreation())
                .Select(w => new SelectableWorkspaceViewModel(w, w.Id == currentWorkspaceId))
                .ToList()
                .AsReadOnly();
        }

        private Task close()
            => navigationService.Close(this, currentWorkspaceId);

        private Task selectWorkspace(SelectableWorkspaceViewModel workspace)
            => navigationService.Close(this, workspace.WorkspaceId);
    }
}
