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

            Title = Intent.ShortcutType().Title();
            Detail = Intent.IntentDescription;

            Type = Intent.ShortcutType();
        }

        public SiriShortcut(SiriShortcutType type)
        {
            Title = type.Title();
            Detail = null;
            InvocationPhrase = null;
            Type = type;
        }

        public static SiriShortcut[] TimerShortcuts = new[]
        {
            new SiriShortcut(
                SiriShortcutType.Start
            ),
            new SiriShortcut(
                SiriShortcutType.Stop
            ),
            new SiriShortcut(
                SiriShortcutType.Continue
            ),
            new SiriShortcut(
                SiriShortcutType.StartFromClipboard
            ),
            new SiriShortcut(
                SiriShortcutType.CustomStart
            )
        };

        public static SiriShortcut[] ReportsShortcuts = new[]
        {
            new SiriShortcut(
                SiriShortcutType.ShowReport
            ),
            new SiriShortcut(
                SiriShortcutType.CustomReport
            )
        };
    }
}
