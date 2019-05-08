using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using Intents;
using IntentsUI;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using ShortcutSection = SectionModel<string, SiriShortcut>;

    public sealed partial class SiriShortcutsViewController : ReactiveViewController<SiriShortcutsViewModel>, IINUIAddVoiceShortcutViewControllerDelegate, IINUIEditVoiceShortcutViewControllerDelegate
    {
        private ISubject<Unit> refreshSubject = new Subject<Unit>();

        public SiriShortcutsViewController() : base(nameof(SiriShortcutsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.Siri_Shortcuts;

            DescriptionLabel.Text = Resources.Siri_Shortcuts_Description;
            HeaderView.RemoveFromSuperview();
            TableView.TableHeaderView = HeaderView;
            HeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            HeaderView.WidthAnchor.ConstraintEqualTo(TableView.WidthAnchor).Active = true;

            var tableViewSource = new SiriShortcutsTableViewSource(TableView);

            TableView.Source = tableViewSource;

            refreshSubject.StartWith(Unit.Default)
                .SelectMany(_ => IosDependencyContainer.Instance.IntentDonationService.GetCurrentShortcuts())
                .Select(toSections)
                .ObserveOn(new NSRunloopScheduler())
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(handleShortcutTap)
                .DisposedBy(DisposeBag);
        }

        private void handleShortcutTap(SiriShortcut shortcut)
        {
            if (shortcut.Identifier == null)
            {
                var intent = IosDependencyContainer.Instance.IntentDonationService.CreateIntent(shortcut.Type);
                var vc = new INUIAddVoiceShortcutViewController(new INShortcut(intent));
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
            else
            {
                var vc = new INUIEditVoiceShortcutViewController(shortcut.VoiceShortcut);
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
        }

        private IEnumerable<ShortcutSection> toSections(IEnumerable<SiriShortcut> shortcuts)
        {
            var defaultShortcuts = SiriShortcut.TimerShortcuts.Concat(SiriShortcut.ReportsShortcuts);

            var allShortcuts = defaultShortcuts.Concat(shortcuts)
                .Aggregate(new List<SiriShortcut>(), (acc, shortcut) =>
                {
                    if (shortcut.Type != SiriShortcutType.CustomStart && shortcut.Type != SiriShortcutType.CustomReport)
                    {
                        var index = acc.IndexOf(s => shortcut.Type == s.Type);
                        if (index != -1)
                        {
                            acc[index] = shortcut;
                            return acc;
                        }
                    }

                    acc.Add(shortcut);
                    return acc;
                });

            return new[]
            {
                new ShortcutSection(
                    "Timer shortcuts",
                    allShortcuts.Where(isTimerShortcut)
                ),
                new ShortcutSection(
                    "Reports shortcuts",
                    allShortcuts.Where(isReportsShortcut)
                )
            };
        }

        private bool isTimerShortcut(SiriShortcut shortcut)
        {
            return shortcut.Type == SiriShortcutType.Stop || shortcut.Type == SiriShortcutType.Start ||
                   shortcut.Type == SiriShortcutType.Continue || shortcut.Type == SiriShortcutType.CustomStart ||
                   shortcut.Type == SiriShortcutType.StartFromClipboard;
        }

        private bool isReportsShortcut(SiriShortcut shortcut)
        {
            return shortcut.Type == SiriShortcutType.ShowReport || shortcut.Type == SiriShortcutType.CustomReport;
        }

        // IINUIAddVoiceShortcutViewControllerDelegate

        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }

        // IINUIEditVoiceShortcutViewControllerDelegate

        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }
    }
}

