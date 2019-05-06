using System;
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
            ViewModel.Shortcuts
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            /*
            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectShortcut.Inputs)
                .DisposedBy(DisposeBag);
            */
        }

        private void generateShortcutList()
        {
            var defaultShortcuts = new[]
            {
                new ShortcutSection(
                    "Timer shortcuts",
                    new[]
                    {
                        new SiriShortcut(
                            "Start empty timer"
                        ),
                        new SiriShortcut(
                            "Stop running entry"
                        ),
                        new SiriShortcut(
                            "Continue last entry"
                        ),
                        new SiriShortcut(
                            "Start custom entry"
                        ),
                    }
                ),
                new ShortcutSection(
                    "Reports shortcuts",
                    new[]
                    {
                        new SiriShortcut(
                            "Show reports"
                        ),
                        new SiriShortcut(
                            "Show custom report"
                        )
                    }
                )
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

