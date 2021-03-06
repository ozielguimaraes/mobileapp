﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using UIKit;
using Toggl.Core.UI.Helper;
using System.Threading.Tasks;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views.Tag;
using System.Reactive;
using System.Linq;
using Toggl.iOS.Presentation.Attributes;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers
{
    [ModalCardPresentation]
    public sealed partial class SelectTagsViewController : KeyboardAwareViewController<SelectTagsViewModel>, IDismissableViewController
    {
        private const double headerHeight = 100;
        private const double placeholderHeight = 250;

        public SelectTagsViewController()
            : base(nameof(SelectTagsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableViewSource = new SelectTagsTableViewSource(TagsTableView);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectTag.Inputs)
                .DisposedBy(DisposeBag);

            var tagsReplay = ViewModel.Tags.Replay();

            tagsReplay
                .Subscribe(TagsTableView.Rx().ReloadItems(tableViewSource))
                .DisposedBy(DisposeBag);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                tagsReplay
                    .Select((tags) =>
                    {
                        var count = tags.ToList().Count();
                        var contentHeight = count > 0
                            ? count * SelectTagsTableViewSource.RowHeight
                            : placeholderHeight;
                        return new CoreGraphics.CGSize(0, contentHeight + headerHeight);
                    })
                    .Subscribe(this.Rx().PreferredContentSize())
                    .DisposedBy(DisposeBag);
            }

            tagsReplay.Connect();

            ViewModel.IsEmpty
                .Subscribe(EmptyStateImage.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsEmpty
                .Subscribe(EmptyStateLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Subscribe(TextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            TextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            BottomConstraint.Active |= UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Pad;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TextField.BecomeFirstResponder();
        }

        public async Task<bool> Dismiss()
        {
            await ViewModel.Close.ExecuteWithCompletion();
            return true;
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = e.FrameEnd.Height;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.Constant = 0;
            UIView.Animate(Animation.Timings.EnterTiming, () => View.LayoutIfNeeded());
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            View.ClipsToBounds |= UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            View.ClipsToBounds |= UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
        }
    }
}
