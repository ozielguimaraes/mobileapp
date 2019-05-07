using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using ShortcutSection = SectionModel<string, SiriShortcut>;

    public sealed partial class SiriShortcutsViewController : ReactiveViewController<SiriShortcutsViewModel>
    {
        public SiriShortcutsViewController() : base(nameof(SiriShortcutsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.Siri_Shortcuts;

            DescriptionLabel.Text = Resources.Siri_Shortcuts_Description;
            HeaderView.RemoveFromSuperview();
            TableView.TableHeaderView = HeaderView;
            HeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            HeaderView.WidthAnchor.ConstraintEqualTo(TableView.WidthAnchor).Active = true;

            var tableViewSource = new SiriShortcutsTableViewSource(TableView);

            TableView.Source = tableViewSource;

            IosDependencyContainer.Instance.IntentDonationService.CurrentShortcuts
                .Select(toSections)
                .ObserveOn(new NSRunloopScheduler())
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            /*
            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectShortcut.Inputs)
                .DisposedBy(DisposeBag);
            */
        }

        private IEnumerable<ShortcutSection> toSections(IEnumerable<SiriShortcut> shortcuts)
        {
            var defaultShortcuts = SiriShortcut.TimerShortcuts.Concat(SiriShortcut.ReportsShortcuts);

            var allShortcuts = defaultShortcuts.Concat(shortcuts)
                .Aggregate(new List<SiriShortcut>(), (acc, shortcut) =>
                {
                    if (shortcut.Type != SiriShortcutType.CustomStart && shortcut.Type != SiriShortcutType.CustomReport)
                    {
                        var index = acc.IndexOf(s => shortcut.Type == s.Type);
                        if (index != -1)
                        {
                            acc[index] = shortcut;
                            return acc;
                        }
                    }

                    acc.Add(shortcut);
                    return acc;
                });

            return new[]
            {
                new ShortcutSection(
                    "Timer shortcuts",
                    allShortcuts.Where(isTimerShortcut)
                ),
                new ShortcutSection(
                    "Reports shortcuts",
                    allShortcuts.Where(isReportsShortcut)
                )
            };
        }

        private bool isTimerShortcut(SiriShortcut shortcut)
        {
            return shortcut.Type == SiriShortcutType.Stop || shortcut.Type == SiriShortcutType.Start ||
                   shortcut.Type == SiriShortcutType.Continue || shortcut.Type == SiriShortcutType.CustomStart ||
                   shortcut.Type == SiriShortcutType.StartFromClipboard;
        }

        private bool isReportsShortcut(SiriShortcut shortcut)
        {
            return shortcut.Type == SiriShortcutType.ShowReport || shortcut.Type == SiriShortcutType.CustomReport;
        }
    }
}

