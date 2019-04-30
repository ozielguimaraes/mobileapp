using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Foundation;
using Intents;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Shared.Models;
using Toggl.Core.Services;
using Toggl.iOS.Intents;
using UIKit;

namespace Toggl.iOS.Services
{
    public class IntentDonationServiceIos : IIntentDonationService
    {
        private IAnalyticsService analyticsService;
        private readonly ITogglDataSource dataSource;

        private INRelevanceProvider[] startTimerRelevanceProviders = {
            new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Work),
            new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Gym),
            new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.School)
        };
        public IntentDonationServiceIos(IAnalyticsService analyticsService, ITogglDataSource dataSource)
        {
            this.analyticsService = analyticsService;
            this.dataSource = dataSource;
        }

        public void SetDefaultShortcutSuggestions(IWorkspace workspace)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            setupDefaultShortcuts(workspace);
        }

        public async Task DonateStartTimeEntry(IWorkspace workspace, ITimeEntry timeEntry)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            var relevantShortcuts = new List<INRelevantShortcut>();

            var startTimerIntent = new StartTimerIntent();
            var startTimerWithClipboardIntent = new StartTimerFromClipboardIntent();
            startTimerIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);
            startTimerWithClipboardIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);

            if (!string.IsNullOrEmpty(timeEntry.Description))
            {
                // If any of the tags or the project id were just created and haven't sync we ignore this action until the user repeats it
                if (timeEntry.ProjectId < 0 || timeEntry.TagIds.Any(tagId => tagId < 0))
                {
                    return;
                }

                if (timeEntry.ProjectId is long projectId)
                {
                    var project = await dataSource.Projects.GetById(projectId).FirstAsync();
                    var projectINObject = new INObject(projectId.ToString(), project.Name);
                    startTimerIntent.ProjectId = projectINObject;
                    startTimerWithClipboardIntent.ProjectId = projectINObject;
                }

                startTimerIntent.EntryDescription = timeEntry.Description;

                var tags = timeEntry.TagIds.Select(tag => new INObject(tag.ToString(), tag.ToString())).ToArray();
                startTimerIntent.Tags = tags;
                startTimerWithClipboardIntent.Tags = tags;

                var billable = new INObject(timeEntry.Billable.ToString(), timeEntry.Billable.ToString());
                startTimerIntent.Billable = billable;
                startTimerWithClipboardIntent.Billable = billable;
                startTimerIntent.SuggestedInvocationPhrase = $"Track {timeEntry.Description}";

                // Relevant shortcut for the Siri Watch Face
                relevantShortcuts.Add(createRelevantShortcut(startTimerIntent));
                relevantShortcuts.Add(createRelevantShortcut(startTimerWithClipboardIntent));
            }
            else
            {
                startTimerIntent.SuggestedInvocationPhrase = Resources.StartTimerInvocationPhrase;
            }

            var startTimerInteraction = new INInteraction(startTimerIntent, null);
            startTimerInteraction.DonateInteraction(trackError);

            var startTimerFromClipboardInteraction = new INInteraction(startTimerWithClipboardIntent, null);
            startTimerFromClipboardInteraction.DonateInteraction(trackError);

            // Descriptionless Relevant Shortcut. Always added even if the intent has one
            var descriptionlessIntent = new StartTimerIntent();
            descriptionlessIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);
            var descriptionlessShortcut = createRelevantShortcut(descriptionlessIntent);
            relevantShortcuts.Add(descriptionlessShortcut);

            donateRelevantShortcuts(relevantShortcuts.ToArray());
        }

        public void DonateStopCurrentTimeEntry()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = Resources.StopTimerInvocationPhrase;

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);

            var shortcut = createRelevantShortcut(intent);
            donateRelevantShortcuts(shortcut);
        }

        public void DonateShowReport(ReportPeriod period)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            var intent = new ShowReportPeriodIntent();
            switch (period)
            {
                case ReportPeriod.Today:
                    intent.Period = ShowReportPeriodReportPeriod.Today;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.Today.ToLower()}'s time";
                    break;
                case ReportPeriod.Yesterday:
                    intent.Period = ShowReportPeriodReportPeriod.Yesterday;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.Yesterday.ToLower()}'s time";
                    break;
                case ReportPeriod.LastWeek:
                    intent.Period = ShowReportPeriodReportPeriod.LastWeek;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.LastWeek.ToLower()}'s time";
                    break;
                case ReportPeriod.LastMonth:
                    intent.Period = ShowReportPeriodReportPeriod.LastMonth;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.LastMonth.ToLower()}'s time";
                    break;
                case ReportPeriod.ThisMonth:
                    intent.Period = ShowReportPeriodReportPeriod.ThisMonth;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.ThisMonth.ToLower()}'s time";
                    break;
                case ReportPeriod.ThisWeek:
                    intent.Period = ShowReportPeriodReportPeriod.ThisWeek;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.ThisWeek.ToLower()}'s time";
                    break;
                case ReportPeriod.ThisYear:
                    intent.Period = ShowReportPeriodReportPeriod.ThisYear;
                    intent.SuggestedInvocationPhrase = $"Show {Resources.ThisYear.ToLower()}'s time";
                    break;
                case ReportPeriod.Unknown:
                    intent.Period = ShowReportPeriodReportPeriod.Unknown;
                    break;
            }

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);
        }

        public void DonateShowReport()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            var intent = new ShowReportIntent();
            intent.SuggestedInvocationPhrase = Resources.ShowReportsInvocationPhrase;
            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);
        }

        public void ClearAll()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                return;
            }

            INInteraction.DeleteAllInteractions(_ => { });
            INVoiceShortcutCenter.SharedCenter.SetShortcutSuggestions(new INShortcut[0]);
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(new INRelevantShortcut[0], trackError);
        }

        private void setupDefaultShortcuts(IWorkspace workspace)
        {
            var startTimerIntent = new StartTimerIntent();
            startTimerIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);
            startTimerIntent.SuggestedInvocationPhrase = Resources.StartTimerInvocationPhrase;
            var startShortcut = new INShortcut(startTimerIntent);
            var startRelevantShorcut = new INRelevantShortcut(startShortcut);
            startRelevantShorcut.RelevanceProviders = startTimerRelevanceProviders;

            var startTimerWithClipboardIntent = new StartTimerFromClipboardIntent();
            startTimerWithClipboardIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);
            var startTimerWithClipboardShortcut = new INShortcut(startTimerWithClipboardIntent);
            var startTimerWithClipboardRelevantShorcut = new INRelevantShortcut(startTimerWithClipboardShortcut);
            startTimerWithClipboardRelevantShorcut.RelevanceProviders = startTimerRelevanceProviders;

            var stopTimerIntent = new StopTimerIntent();
            stopTimerIntent.SuggestedInvocationPhrase = Resources.StopTimerInvocationPhrase;
            var stopShortcut = new INShortcut(stopTimerIntent);
            var stopRelevantShortcut = new INRelevantShortcut(stopShortcut);
            stopRelevantShortcut.RelevanceProviders = new[]
            {
                new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Home)
            };

            var showReportIntent = new ShowReportIntent();
            showReportIntent.SuggestedInvocationPhrase = Resources.ShowReportsInvocationPhrase;
            var reportShortcut = new INShortcut(showReportIntent);

            var continueTimerIntent = new ContinueTimerIntent
            {
                SuggestedInvocationPhrase = Resources.ContinueTimerInvocationPhrase
            };
            continueTimerIntent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);
            var continueTimerShortcut = new INShortcut(continueTimerIntent);
            var continueTimerRelevantShortcut = new INRelevantShortcut(continueTimerShortcut)
            {
                RelevanceProviders = startTimerRelevanceProviders
            };

            var shortcuts = new[] { startShortcut, stopShortcut, reportShortcut, continueTimerShortcut, startTimerWithClipboardShortcut };
            INVoiceShortcutCenter.SharedCenter.SetShortcutSuggestions(shortcuts);

            var relevantShortcuts = new[] { startRelevantShorcut, stopRelevantShortcut, continueTimerRelevantShortcut, startTimerWithClipboardRelevantShorcut };
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(relevantShortcuts, trackError);
        }

        private INRelevantShortcut createRelevantShortcut(INIntent intent)
        {
            var shortcut = new INShortcut(intent);
            var relevantShortcut = new INRelevantShortcut(shortcut);
            relevantShortcut.RelevanceProviders = new List<INRelevanceProvider>().ToArray();
            return relevantShortcut;
        }

        private void donateRelevantShortcuts(params INRelevantShortcut[] relevantShortcuts)
        {
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(relevantShortcuts, trackError);
        }

        private void trackError(NSError error)
        {
            if (error == null)
                return;

            analyticsService.TrackAnonymized(new Exception(error.LocalizedDescription));
        }
    }
}
