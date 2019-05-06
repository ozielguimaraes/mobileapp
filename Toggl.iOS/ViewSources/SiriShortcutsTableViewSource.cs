using System;
using Foundation;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Cells;
using Toggl.iOS.Views.Settings;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using SiriShortcutsSection = SectionModel<string, SiriShortcutViewModel>;

    public class SiriShortcutsTableViewSource : BaseTableViewSource<SiriShortcutsSection, string, SiriShortcutViewModel>
    {
        public SiriShortcutsTableViewSource(UITableView tableView)
        {
            tableView.RegisterNibForCellReuse(SiriShortcutCell.Nib, SiriShortcutCell.Identifier);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            // var identifier = model is SelectableClientCreationViewModel ? CreateClientViewCell.Identifier : ClientViewCell.Identifier;
            var identifier = SiriShortcutCell.Identifier;
            var cell = (BaseTableViewCell<SiriShortcutViewModel>)tableView.DequeueReusableCell(identifier, indexPath);
            cell.Item = model;
            return cell;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return "Header";
        }
    }
}
