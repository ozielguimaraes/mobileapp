using System;
using Foundation;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Cells;
using Toggl.iOS.Models;
using Toggl.iOS.Views.Settings;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using SiriShortcutsSection = SectionModel<string, SiriShortcut>;

    public class SiriShortcutsTableViewSource : BaseTableViewSource<SiriShortcutsSection, string, SiriShortcut>
    {
        public SiriShortcutsTableViewSource(UITableView tableView)
        {
            tableView.RegisterNibForCellReuse(SiriShortcutCell.Nib, SiriShortcutCell.Identifier);
            tableView.RegisterNibForCellReuse(ExistingSiriShortcutCell.Nib, ExistingSiriShortcutCell.Identifier);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);

            var identifier = model.Identifier == null
                ? SiriShortcutCell.Identifier
                : ExistingSiriShortcutCell.Identifier;

            var cell = (BaseTableViewCell<SiriShortcut>)tableView.DequeueReusableCell(identifier, indexPath);
            cell.Item = model;
            return cell;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return HeaderOf(section);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            return model.Identifier == null ? 44 : 64;
        }
    }
}
