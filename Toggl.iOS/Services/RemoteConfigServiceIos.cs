﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Firebase.RemoteConfig;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class RemoteConfigServiceIos : IRemoteConfigService
    {
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";

        public RemoteConfigServiceIos()
        {
            var remoteConfig = RemoteConfig.SharedInstance;
            remoteConfig.SetDefaults(plistFileName: remoteConfigDefaultsFileName);
        }

        public IObservable<RatingViewConfiguration> RatingViewConfiguration
            => Observable.Create<RatingViewConfiguration>(observer =>
            {
                var remoteConfig = RemoteConfig.SharedInstance;
                remoteConfig.Fetch((status, error) =>
                {
                    if (error == null)
                        remoteConfig.ActivateFetched();

                    var configuration = new RatingViewConfiguration(
                        remoteConfig["day_count"].NumberValue.Int32Value,
                        remoteConfig["criterion"].StringValue.ToRatingViewCriterion()
                    );
                    observer.OnNext(configuration);
                    observer.OnCompleted();
                });
                return Disposable.Empty;
            });
    }
}
