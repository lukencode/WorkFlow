using Nevermore;
using Nevermore.Mapping;
using Nevermore.RelatedDocuments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Model;
using WorkFlow.Sql.Model;
using WorkFlow.SQL;

namespace WorkFlow.Sql
{
    public class WorkFlowSqlStorage : IWorkFlowStorage
    {
        private string connectionString;
        private string appName;
        private RelationalStore store;

        private bool autoMigrations;

        public WorkFlowSqlStorage(string connectionString, string appName, bool autoMigrations = true)
        {
            this.connectionString = connectionString;
            this.appName = appName;
            this.autoMigrations = autoMigrations;
        }

        private void InitStore()
        {
            if (store != null) return;
            if(autoMigrations) new Migrator(connectionString).Upgrade();

            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            };
            
            var mappings = new RelationalMappings();
            mappings.Install(new[] { new StoredWorkFlowStateMap() });

            store = new RelationalStore(
                connectionString,
                appName,
                new SqlCommandFactory(),
                mappings,
                jsonSettings,
                new EmptyRelatedDocumentStore()
            );
        }

        public WorkFlowState Load(string id)
        {
            InitStore();

            using (var trn = store.BeginTransaction())
            {
                var workflowstate = trn.Load<StoredWorkFlowState>(id);
                return workflowstate as WorkFlowState;
            }
        }

        public void Save(WorkFlowState state)
        {
            InitStore();

            using (var trn = store.BeginTransaction(RetriableOperation.Update))
            {
                var isExistingWorkflow = false;
                var storedState = (StoredWorkFlowState)state;

                if (!isExistingWorkflow)
                {
                    trn.Insert(storedState);
                }
                else
                {
                    trn.Update(storedState);
                }

                trn.Commit();
            }
        }
    }
}
