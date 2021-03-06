﻿using System;

namespace Toggl.Core.UI.Services
{
    public interface IPermissionsService
    {
        IObservable<bool> CalendarPermissionGranted { get; }

        IObservable<bool> NotificationPermissionGranted { get; }

        IObservable<bool> RequestCalendarAuthorization(bool force = false);

        IObservable<bool> RequestNotificationAuthorization(bool force = false);

        void OpenAppSettings();
    }
}
