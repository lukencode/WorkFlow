using DbUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.SQL
{
    public class Migrator
    {
        private string connectionString;

        internal Migrator(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Upgrade()
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .JournalToSqlTable("dbo", "WorkFlowJournal")
                    .LogToConsole()
                    .Build();
        }
    }
}
