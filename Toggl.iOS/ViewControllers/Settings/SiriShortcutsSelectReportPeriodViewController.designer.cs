// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.ViewControllers.Settings
{
	[Register ("SiriShortcutsSelectReportPeriodViewController")]
	partial class SiriShortcutsSelectReportPeriodViewController
	{
		[Outlet]
		UIKit.UIView AddToSiriWrapperView { get; set; }

		[Outlet]
		UIKit.UIButton BackButton { get; set; }

		[Outlet]
		UIKit.UITableView TableView { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}

			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (AddToSiriWrapperView != null) {
				AddToSiriWrapperView.Dispose ();
				AddToSiriWrapperView = null;
			}
		}
	}
}
