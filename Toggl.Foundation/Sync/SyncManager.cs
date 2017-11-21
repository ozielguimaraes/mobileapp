﻿using System;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using static Toggl.Foundation.Sync.SyncState;

namespace Toggl.Foundation.Sync
{
    public sealed class SyncManager : ISyncManager
    {
        private readonly object stateLock = new object();
        private readonly ISyncStateQueue queue;
        private readonly IStateMachineOrchestrator orchestrator;

        private bool isFrozen;

        public bool IsRunningSync { get; private set; }

        public SyncState State => orchestrator.State;
        public IObservable<SyncState> StateObservable => orchestrator.StateObservable;

        public SyncManager(ISyncStateQueue queue, IStateMachineOrchestrator orchestrator)
        {
            Ensure.Argument.IsNotNull(queue, nameof(queue));
            Ensure.Argument.IsNotNull(orchestrator, nameof(orchestrator));

            this.queue = queue;
            this.orchestrator = orchestrator;

            orchestrator.SyncCompleteObservable.Subscribe(syncOperationCompleted);
            isFrozen = false;
        }

        public IObservable<SyncState> PushSync()
        {
            lock (stateLock)
            {
                queue.QueuePushSync();
                return startSyncIfNeededAndObserve();
            }
        }

        public IObservable<SyncState> ForceFullSync()
        {
            lock (stateLock)
            {
                queue.QueuePullSync();
                return startSyncIfNeededAndObserve();
            }
        }

        public IObservable<SyncState> Freeze()
        {
            lock (stateLock)
            {
                if (isFrozen == false)
                {
                    isFrozen = true;
                    orchestrator.Freeze();
                }

                return IsRunningSync
                    ? syncStatesUntilAndIncludingSleep().LastAsync()
                    : Observable.Return(Sleep);
            }
        }

        private void syncOperationCompleted(SyncResult result)
        {
            lock (stateLock)
            {
                IsRunningSync = false;

                if (result is Success)
                {
                    startSyncIfNeeded();
                    return;
                }

                if (result is Error)
                {
                    orchestrator.Start(Sleep);
                    return;
                }

                throw new ArgumentException(nameof(result));
            }
        }

        private IObservable<SyncState> startSyncIfNeededAndObserve()
        {
            startSyncIfNeeded();

            return syncStatesUntilAndIncludingSleep();
        }

        private void startSyncIfNeeded()
        {
            if (IsRunningSync) return;

            var state = isFrozen ? Sleep : queue.Dequeue();
            IsRunningSync = state != Sleep;
            orchestrator.Start(state);
        }

        private IObservable<SyncState> syncStatesUntilAndIncludingSleep()
            => StateObservable.TakeWhile(s => s != Sleep)
                .Concat(Observable.Return(Sleep))
                .ConnectedReplay();
    }
}
