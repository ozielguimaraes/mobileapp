﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Realms;
using Remotion.Linq.Clauses;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;
using Toggl.Storage.Realm.Models;

namespace Toggl.Storage.Realm
{
    public sealed class Database : ITogglDatabase
    {
        private readonly RealmConfiguration realmConfiguration;

        public Database()
        {
            realmConfiguration = createRealmConfiguration();
            IdProvider = new IdProvider(getRealmInstance);
            SinceParameters = createSinceParameterRepository();
            Tags = Repository<IDatabaseTag>.For(getRealmInstance, (tag, realm) => new RealmTag(tag, realm));
            Tasks = Repository<IDatabaseTask>.For(getRealmInstance, (task, realm) => new RealmTask(task, realm));
            User = SingleObjectStorage<IDatabaseUser>.For(getRealmInstance, (user, realm) => new RealmUser(user, realm));
            Clients = Repository<IDatabaseClient>.For(getRealmInstance, (client, realm) => new RealmClient(client, realm));
            Preferences = SingleObjectStorage<IDatabasePreferences>.For(getRealmInstance, (preferences, realm) => new RealmPreferences(preferences, realm));
            Projects = Repository<IDatabaseProject>.For(getRealmInstance, (project, realm) => new RealmProject(project, realm));
            TimeEntries = Repository<IDatabaseTimeEntry>.For(getRealmInstance, (timeEntry, realm) => new RealmTimeEntry(timeEntry, realm));
            Workspaces = Repository<IDatabaseWorkspace>.For(getRealmInstance, (workspace, realm) => new RealmWorkspace(workspace, realm));
            WorkspaceFeatures = Repository<IDatabaseWorkspaceFeatureCollection>.For(
                getRealmInstance,
                (collection, realm) => new RealmWorkspaceFeatureCollection(collection, realm),
                id => x => x.WorkspaceId == id,
                ids => x => ids.Contains(x.WorkspaceId),
                features => features.WorkspaceId);
        }

        public IIdProvider IdProvider { get; }
        public ISinceParameterRepository SinceParameters { get; }
        public IRepository<IDatabaseTag> Tags { get; }
        public IRepository<IDatabaseTask> Tasks { get; }
        public IRepository<IDatabaseClient> Clients { get; }
        public ISingleObjectStorage<IDatabasePreferences> Preferences { get; }
        public IRepository<IDatabaseProject> Projects { get; }
        public ISingleObjectStorage<IDatabaseUser> User { get; }
        public IRepository<IDatabaseTimeEntry> TimeEntries { get; }
        public IRepository<IDatabaseWorkspace> Workspaces { get; }
        public IRepository<IDatabaseWorkspaceFeatureCollection> WorkspaceFeatures { get; }

        public IObservable<Unit> Clear() =>
            Observable.Start(() =>
            {
                var realm = getRealmInstance();

                using (var transaction = realm.BeginWrite())
                {
                    realm.RemoveAll();
                    transaction.Commit();
                }
            });

        private Realms.Realm getRealmInstance()
            => Realms.Realm.GetInstance(realmConfiguration);

        private ISinceParameterRepository createSinceParameterRepository()
        {
            var sinceParametersRealmAdapter =
                new RealmAdapter<RealmSinceParameter, IDatabaseSinceParameter>(
                    getRealmInstance,
                    (parameter, realm) => new RealmSinceParameter(parameter),
                    id => entity => entity.Id == id,
                    ids => entity => ids.Contains(entity.Id),
                    parameter => parameter.Id);

            return new SinceParameterStorage(sinceParametersRealmAdapter);
        }

        private RealmConfiguration createRealmConfiguration()
            => new RealmConfiguration
            {
                SchemaVersion = 8,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                    if (oldSchemaVersion < 3)
                    {
                        // nothing needs explicit updating when updating from schema 0 up to 3
                    }

                    if (oldSchemaVersion < 4)
                    {
                        var newTags = migration.NewRealm.All<RealmTag>();
                        var oldTags = migration.OldRealm.All("RealmTag");
                        for (var i = 0; i < newTags.Count(); i++)
                        {
                            var oldTag = oldTags.ElementAt(i);
                            var newTag = newTags.ElementAt(i);
                            newTag.ServerDeletedAt = oldTag.DeletedAt;
                        }
                    }

                    if (oldSchemaVersion < 6)
                    {
                        // nothing needs explicit updating when updating from schema 4 up to 6
                    }

                    if (oldSchemaVersion < 7)
                    {
                        // RealmWorkspace: IsGhost was renamed to IsInaccessible
                        // A migration is not required because the property was not used until now
                    }

                    if (oldSchemaVersion < 8)
                    {
                        // RealmUser: Added new property
                        // A migration is not required because the value will be filled in on next
                        migration.NewRealm.All<RealmUser>().Do(user => user.Timezone = string.Empty);
                    }
                }
            };
    }
}
