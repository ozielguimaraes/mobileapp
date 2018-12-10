using System;
using Toggl.Foundation.Helper;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Suggestions
{
    [Preserve(AllMembers = true)]
    public sealed class Suggestion : ITimeEntryPrototype
    {
        public string Description { get; } = "";

        public long? ProjectId { get; } = null;

        public long? TaskId { get; } = null;

        public long? ClientId { get; } = null;

        public string ProjectColor { get; } = Color.NoProject;

        public string ProjectName { get; } = "";

        public string TaskName { get; } = "";

        public string ClientName { get; } = "";

        public bool HasProject { get; } = false;

        public long WorkspaceId { get; }

        public bool IsBillable { get; } = false;

        public long[] TagIds { get; } = Array.Empty<long>();

        public DateTimeOffset StartTime { get; }

        public TimeSpan? Duration { get; } = null;

        public float Certainty { get; }

        internal Suggestion(IDatabaseTimeEntry timeEntry, float certainty)
        {
            Ensure.Argument.IsInClosedRange(certainty, 0, 1, nameof(certainty));

            Certainty = certainty;
            TaskId = timeEntry.TaskId;
            ProjectId = timeEntry.ProjectId;
            IsBillable = timeEntry.Billable;
            Description = timeEntry.Description;
            WorkspaceId = timeEntry.WorkspaceId;

            if (timeEntry.Project == null) return;

            HasProject = true;
            ProjectName = timeEntry.Project.Name;
            ProjectColor = timeEntry.Project.Color;

            ClientName = timeEntry.Project.Client?.Name ?? "";
            ClientId = timeEntry.Project.ClientId;

            if (timeEntry.Task == null) return;

            TaskName = timeEntry.Task.Name;
        }
    }
}
