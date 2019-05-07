using System;
using Intents;
using Toggl.iOS.Intents;

namespace Toggl.iOS.Models
{
    public enum SiriShortcutType
    {
        Start,
        StartFromClipboard,
        Continue,
        Stop,
        CustomStart,
        ShowReport,
        CustomReport
    }

    static class INIntentExtensions
    {
        public static SiriShortcutType ShortcutType(this INIntent intent)
        {
            if (intent is StartTimerIntent startTimerIntent)
            {
                if (startTimerIntent.EntryDescription != null ||
                    startTimerIntent.Tags != null ||
                    startTimerIntent.ProjectId != null)
                    return SiriShortcutType.CustomStart;

                return SiriShortcutType.Start;
            }

            if (intent is StartTimerFromClipboardIntent)
                return SiriShortcutType.StartFromClipboard;

            if (intent is StopTimerIntent)
                return SiriShortcutType.Stop;

            if (intent is ContinueTimerIntent)
                return SiriShortcutType.Continue;

            if (intent is ShowReportIntent)
                return SiriShortcutType.ShowReport;

            if (intent is ShowReportPeriodIntent)
                return SiriShortcutType.CustomReport;

            throw new ArgumentOutOfRangeException("intent");
        }
    }
}
