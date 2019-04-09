﻿using System;
using Toggl.Core.Services;

namespace Toggl.Giskard.Services
{
    public class NoopPrivateSharedStorageServiceAndroid : IPrivateSharedStorageService
    {
        public void SaveApiToken(string apiToken)
        {
        }

        public void SaveUserId(long userId)
        {
        }

        public void SaveLastUpdateDate(DateTimeOffset date)
        {
        }

        public void ClearAll()
        {
        }
    }
}
