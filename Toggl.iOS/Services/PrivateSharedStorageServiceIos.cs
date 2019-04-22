﻿using System;
using Toggl.Core.Services;
using Foundation;
using Toggl.iOS.ExtensionKit;

namespace Toggl.iOS.Services
{
    public class PrivateSharedStorageServiceIos : IPrivateSharedStorageService
    {
        public void SaveApiToken(string apiToken)
        {
            SharedStorage.instance.SetApiToken(apiToken);
        }

        public void SaveUserId(long userId)
        {
            SharedStorage.instance.SetUserId(userId);
        }

        public void SaveLastUpdateDate(DateTimeOffset date)
        {
            SharedStorage.instance.SetLastUpdateDate(date);
        }

        public void ClearAll()
        {
            SharedStorage.instance.DeleteEverything();
        }
    }
}
