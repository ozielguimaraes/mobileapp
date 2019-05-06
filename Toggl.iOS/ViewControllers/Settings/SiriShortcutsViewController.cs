using System;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.ViewSources;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
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
/*
            TableView.Source = tableViewSource;
            ViewModel.Shortcuts
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectShortcut.Inputs)
                .DisposedBy(DisposeBag);*/
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

