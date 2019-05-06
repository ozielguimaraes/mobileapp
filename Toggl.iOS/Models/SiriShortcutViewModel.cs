using System.Collections.Generic;

namespace Toggl.iOS.Models
{
    public class SiriShortcut
    {
        public string Title { get; }

        public string IntentIdentifier { get; }
        public string InvocationPhrase { get; }
        public Dictionary<string, string> Parameters { get; }

        public SiriShortcut(string title, string intentIdentifier = null, string invocationPhrase = null, Dictionary<string, string> parameters = null)
        {
            Title = title;
            IntentIdentifier = intentIdentifier;
            InvocationPhrase = invocationPhrase;
            Parameters = parameters;
        }
    }
}
