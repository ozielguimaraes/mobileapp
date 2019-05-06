using System.Collections.Generic;

namespace Toggl.Core.UI.ViewModels.Settings
{
    public class SiriShortcutViewModel
    {
        public string Title { get; }

        public string IntentIdentifier { get; }
        public string InvocationPhrase { get; }
        public Dictionary<string, string> Parameters { get; }

        public SiriShortcutViewModel(string title, string intentIdentifier = null, string invocationPhrase = null, Dictionary<string, string> parameters = null)
        {
            Title = title;
            IntentIdentifier = intentIdentifier;
            InvocationPhrase = invocationPhrase;
            Parameters = parameters;
        }
    }
}
