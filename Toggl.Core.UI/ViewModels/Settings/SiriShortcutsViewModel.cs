using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Services;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SiriShortcutsViewModel : ViewModel
    {
        public IObservable<IEnumerable<SectionModel<string, SiriShortcutViewModel>>> Shortcuts { get; }
        public InputAction<SiriShortcutViewModel> SelectShortcut { get; }

        public SiriShortcutsViewModel(ISchedulerProvider schedulerProvider, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            SelectShortcut = rxActionFactory.FromAction<SiriShortcutViewModel>(selectShortcut);

            var defaultShortcuts = new[]
            {
                new SectionModel<string, SiriShortcutViewModel>(
                    "Timer shortcuts",
                    new[]
                    {
                        new SiriShortcutViewModel(
                            "Start empty timer"
                        ),
                        new SiriShortcutViewModel(
                            "Stop running entry"
                        ),
                        new SiriShortcutViewModel(
                            "Continue last entry"
                        ),
                        new SiriShortcutViewModel(
                            "Start custom entry"
                        ),
                    }
                ),
                new SectionModel<string, SiriShortcutViewModel>(
                    "Reports shortcuts",
                    new[]
                    {
                        new SiriShortcutViewModel(
                            "Show reports"
                        ),
                        new SiriShortcutViewModel(
                            "Show custom report"
                        )
                    }
                )
            };

            Shortcuts = Observable.Return(defaultShortcuts)
                .AsDriver(schedulerProvider);
        }

        private void selectShortcut(SiriShortcutViewModel shortcut)
        {
            throw new NotImplementedException();
        }
    }
}
