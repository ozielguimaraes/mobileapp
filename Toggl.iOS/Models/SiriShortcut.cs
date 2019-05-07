using System;
using System.Collections.Generic;
using Intents;
using Toggl.iOS.Services;

namespace Toggl.iOS.Models
{
    public class SiriShortcut
    {
        public string Title { get; }
        public string Detail { get; }
        public string InvocationPhrase { get; }
        public SiriShortcutType Type { get; }
        public string Identifier { get; }

        private INIntent Intent { get; }

        public SiriShortcut(INVoiceShortcut voiceShortcut)
        {
            Identifier = voiceShortcut.Identifier.AsString();
            InvocationPhrase = voiceShortcut.InvocationPhrase;
            Intent = voiceShortcut.Shortcut.Intent;

            Title = Intent.SuggestedInvocationPhrase;
            Detail = Intent.IntentDescription;

            Type = Intent.ShortcutType();
        }

        public SiriShortcut(string title, SiriShortcutType type)
        {
            Title = title;
            Detail = null;
            InvocationPhrase = null;
            Type = type;
        }

        public static SiriShortcut[] TimerShortcuts = new[]
        {
            new SiriShortcut(
                "Start empty timer",
                SiriShortcutType.Start
            ),
            new SiriShortcut(
                "Stop running entry",
                SiriShortcutType.Stop
            ),
            new SiriShortcut(
                "Continue last entry",
                SiriShortcutType.Continue
            ),
            new SiriShortcut(
                "Start empty from clipboard",
                SiriShortcutType.StartFromClipboard
            ),
            new SiriShortcut(
                "Start custom entry",
                SiriShortcutType.CustomStart
            )
        };

        public static SiriShortcut[] ReportsShortcuts = new[]
        {
            new SiriShortcut(
                "Show reports",
                SiriShortcutType.ShowReport
            ),
            new SiriShortcut(
                "Show custom report",
                SiriShortcutType.CustomReport
            )
        };
    }
}
