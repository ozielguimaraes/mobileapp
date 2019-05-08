﻿using System;

using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Models;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class SiriShortcutCell : BaseTableViewCell<SiriShortcut>
    {
        public static readonly string Identifier = nameof(SiriShortcutCell);
        public static readonly UINib Nib;

        static SiriShortcutCell()
        {
            Nib = UINib.FromName("SiriShortcutCell", NSBundle.MainBundle);
        }

        protected SiriShortcutCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
            DetailLabel.Text = "Add";
            DetailLabel.TextColor = Colors.Siri.AddButton.ToNativeColor();
        }
    }
}

