﻿using System.Reactive;
using UIKit;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Core.MvvmCross.ViewModels;
using Toggl.Daneel.Cells;
using Toggl.Core.Models;
using Toggl.Daneel.ViewSources.Generic.TableView;
using Toggl.Core.MvvmCross.Collections;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed class SyncFailuresViewController : MvxTableViewController<SyncFailuresViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.RegisterNibForCellReuse(SyncFailureCell.Nib, SyncFailureCell.Identifier);
            var tableViewSource = new CustomTableViewSource<SectionModel<Unit, SyncFailureItem>, Unit, SyncFailureItem>(
                SyncFailureCell.CellConfiguration(SyncFailureCell.Identifier),
                ViewModel.SyncFailures
            );
            TableView.Source = tableViewSource;
        }
    }
}
