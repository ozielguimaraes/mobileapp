using System;

using Foundation;
using Toggl.iOS.Cells;
using Toggl.iOS.Models;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class ExistingSiriShortcutCell : BaseTableViewCell<SiriShortcut>
    {
        public static readonly string Identifier = nameof(ExistingSiriShortcutCell);
        public static readonly UINib Nib;

        static ExistingSiriShortcutCell()
        {
            Nib = UINib.FromName("ExistingSiriShortcutCell", NSBundle.MainBundle);
        }

        protected ExistingSiriShortcutCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
            DetailsLabel.Text = Item.Detail;
            InvocationLabel.Text = Item.InvocationPhrase;
        }
    }
}

