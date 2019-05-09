using System;
using System.Reactive;
using System.Reactive.Linq;
using IntentsUI;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
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

            prepareSiriButton();

            TitleLabel.Text = Resources.ReportPeriod;
            SelectWorkspaceCellLabel.Text = Resources.SelectWorkspace;

            TableView.RegisterNibForCellReuse(SiriShortcutReportPeriodCell.Nib, SiriShortcutReportPeriodCell.Identifier);

            TableView.RowHeight = rowHeight;

            var source =
                new CustomTableViewSource<SectionModel<Unit, SelectableReportPeriodViewModel>, Unit,
                    SelectableReportPeriodViewModel>(
                    SiriShortcutReportPeriodCell.CellConfiguration(SiriShortcutReportPeriodCell.Identifier)
                    );

            ViewModel.ReportPeriods
                .Subscribe(TableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            source
                .Rx()
                .ModelSelected()
                .Select(p => p.ReportPeriod)
                .Subscribe(ViewModel.SelectReportPeriod.Accept)
                .DisposedBy(DisposeBag);

            TableView.Source = source;

            BackButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            SelectWorkspaceView.Rx()
                .BindAction(ViewModel.PickWorkspace)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(SelectWorkspaceNameLabel.Rx().Text())
                .DisposedBy(DisposeBag);
        }

        private void prepareSiriButton()
        {
            var button = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.Black);
            button.TranslatesAutoresizingMaskIntoConstraints = false;

            var descriptionLabel = new UILabel
            {
                Text = Resources.SiriReportPeriodInstruction,
                Font = UIFont.SystemFontOfSize(12),
                TextColor = Colors.Siri.InvocationPhrase.ToNativeColor()
            };
            descriptionLabel.TranslatesAutoresizingMaskIntoConstraints = false;

            AddToSiriWrapperView.AddSubview(button);
            AddToSiriWrapperView.AddSubview(descriptionLabel);

            NSLayoutConstraint.ActivateConstraints(new []
            {
                descriptionLabel.CenterXAnchor.ConstraintEqualTo(AddToSiriWrapperView.CenterXAnchor),
                descriptionLabel.TopAnchor.ConstraintEqualTo(AddToSiriWrapperView.TopAnchor, 16),
                button.CenterXAnchor.ConstraintEqualTo(AddToSiriWrapperView.CenterXAnchor),
                button.TopAnchor.ConstraintEqualTo(descriptionLabel.BottomAnchor, 16),
            });

            button.TouchUpInside += siriButtonHandler;
        }

        private void siriButtonHandler(object sender, EventArgs args)
        {
            //TODO: Create custom shortcuts
        }
    }
}

