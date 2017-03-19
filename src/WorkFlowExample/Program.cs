using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkFlow;
using WorkFlow.Model;
using WorkFlow.Sql;

namespace WorkFlowExample
{
    class Program
    {
        //$"DataSource={Path.Combine(path, $"{appName}.sdf")}";
        //Data Source=(LocalDB)\v11.0;AttachDbFileName=|DataDirectory|\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True
        static string connectionString => $@"Data Source=(LocalDB)\v11.0;AttachDbFileName={path}\{appName}.mdf;Integrated Security=True;MultipleActiveResultSets=True";
        static string appName = "WorkFlowTest";

        static IWorkFlowStorage storage;

        static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

        static void Main(string[] args)
        {
            storage = new WorkFlowSqlStorage(connectionString, appName);

            var workflow = GetNewWorkFlow();
            workflow.Run();

            Console.WriteLine("Result: " + workflow.Status);
            Console.ReadLine();
        }

        private static WorkFlowState GetNewWorkFlow()
        {
            var workflow = new WorkFlowState(storage);

            workflow.Steps.Add(new Step()
            {
                Name = "Procurement Review",
                StartCondition = StartCondition.Any,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                Actions = new List<WorkFlow.Model.Action>()
                {
                    new WaitReadLineStep() { Name = "Procurement Approval" }
                }
            });
            
            workflow.Steps.Add(new Step()
            {
                Name = "Financial Review",
                StartCondition = StartCondition.Any,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                Actions = new List<WorkFlow.Model.Action>()
                {
                    new WaitReadLineStep() { Name = "Financial Delegation Approval" }
                }
            });

            workflow.Steps.Add(new Step()
            {
                Name = "Procurment Release",
                StartCondition = StartCondition.Success,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                Actions = new List<WorkFlow.Model.Action>()
                {
                    new WaitReadLineStep() { Name = "Procurement Approval" }
                }
            });


            return workflow;
        }
    }
}
