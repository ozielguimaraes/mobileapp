﻿using System;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectDateTimeViewModel : ViewModel<DateTimePickerParameters, DateTimeOffset>
    {
        private DateTimeOffset defaultResult;
        private readonly INavigationService navigationService;

        public DateTimeOffset MinDate { get; private set; }
        public DateTimeOffset MaxDate { get; private set; }
        public DateTimePickerMode Mode { get; private set; }
        public bool Is24HoursFormat { get; private set; } = true;
        public BehaviorRelay<DateTimeOffset> CurrentDateTime { get; private set; }

        public UIAction CloseCommand { get; }
        public UIAction SaveCommand { get; }

        public SelectDateTimeViewModel(IRxActionFactory rxActionFactory, INavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            SaveCommand = rxActionFactory.FromAsync(save);
            CloseCommand = rxActionFactory.FromAsync(close);
        }

        public override void Prepare(DateTimePickerParameters parameter)
        {
            Mode = parameter.Mode;
            MinDate = parameter.MinDate;
            MaxDate = parameter.MaxDate;
            CurrentDateTime = new BehaviorRelay<DateTimeOffset>(parameter.CurrentDate, sanitizeBasedOnMode);
        }

        private DateTimeOffset sanitizeBasedOnMode(DateTimeOffset dateTime)
        {
            var result = DateTimeOffset.MinValue;

            switch (Mode)
            {
                case DateTimePickerMode.Date:
                    result = defaultResult.ToUniversalTime().WithDate(dateTime.ToUniversalTime());
                    break;

                case DateTimePickerMode.Time:
                    result = defaultResult.ToUniversalTime().WithTime(dateTime.ToUniversalTime());
                    break;

                case DateTimePickerMode.DateTime:
                    result = dateTime;
                    break;
                
                default:
                    throw new NotSupportedException("Invalid DateTimePicker mode");
            }

            return result.Clamp(MinDate, MaxDate);
        }

        private Task close() => navigationService.Close(this, defaultResult);

        private Task save() => navigationService.Close(this, CurrentDateTime.Value);
    }
}
