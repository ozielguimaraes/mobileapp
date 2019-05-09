using System;
using System.Reactive;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Presentation.Attributes;
using Toggl.iOS.Views.Settings;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    [ModalCardPresentation]
    public partial class SiriShortcutsSelectReportPeriodViewController : ReactiveViewController<SiriShortcutsSelectReportPeriodViewModel>
    {
        private const int rowHeight = 48;

        public SiriShortcutsSelectReportPeriodViewController()
            : base(nameof(SiriShortcutsSelectReportPeriodViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.ReportPeriod;

            TableView.RegisterNibForCellReuse(SiriShortcutReportPeriodCell.Nib, SiriShortcutReportPeriodCell.Identifier);

            TableView.RowHeight = rowHeight;

            var source =
                new CustomTableViewSource<SectionModel<Unit, SelectableReportPeriodViewModel>, Unit,
                    SelectableReportPeriodViewModel>(
                    SiriShortcutReportPeriodCell.CellConfiguration(SiriShortcutReportPeriodCell.Identifier),
                    ViewModel.ReportPeriods);

            source.Rx().ModelSelected().Debug("SELECTED").Subscribe().DisposedBy(DisposeBag);

            TableView.Source = source;

            BackButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);
        }
    }
}

