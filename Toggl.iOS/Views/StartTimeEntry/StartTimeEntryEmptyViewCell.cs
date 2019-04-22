﻿using System;
using Foundation;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.UI.Helper;
using UIKit;
using Toggl.iOS.Extensions;
using Toggl.iOS.Cells;

namespace Toggl.iOS.Views
{
    public partial class StartTimeEntryEmptyViewCell : BaseTableViewCell<QuerySymbolSuggestion>
    {
        public static readonly string Identifier = nameof(StartTimeEntryEmptyViewCell);
        public static readonly UINib Nib;

        static StartTimeEntryEmptyViewCell()
        {
            Nib = UINib.FromName(nameof(StartTimeEntryEmptyViewCell), NSBundle.MainBundle);
        }

        protected StartTimeEntryEmptyViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            DescriptionLabel.AttributedText = suggestionToAttributedString(Item);
        }

        private NSAttributedString suggestionToAttributedString(QuerySymbolSuggestion value)
        {
            var a = $"{value.Symbol} {value.Description}";
            var result = new NSMutableAttributedString(a);
            result.AddAttributes(new UIStringAttributes
            {
                Font = UIFont.BoldSystemFontOfSize(16),
                ForegroundColor = Colors.StartTimeEntry.BoldQuerySuggestionColor.ToNativeColor()
            }, new NSRange(0, 1));

            result.AddAttributes(new UIStringAttributes
            {
                Font = UIFont.SystemFontOfSize(13),
                ForegroundColor = Colors.StartTimeEntry.Placeholder.ToNativeColor()
            }, new NSRange(2, value.Description.Length));

            return result;
        }
    }
}
