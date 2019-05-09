using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SiriShortcutsViewModel : ViewModel
    {
        private readonly INavigationService navigationService;
        public UIAction NavigateToCustomReportShortcut;

        public SiriShortcutsViewModel(
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;

            NavigateToCustomReportShortcut = rxActionFactory.FromAsync(navigateToCustomReportShortcut);
        }

        private Task navigateToCustomReportShortcut() => navigationService.Navigate<SiriShortcutsSelectReportPeriodViewModel>();
    }
}
