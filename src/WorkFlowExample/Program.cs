using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow;
using WorkFlow.Model;

namespace WorkFlowExample
{
    class Program
    {
        static IWorkFlowStorage storage => new MemoryWorkflowStorage();

        static void Main(string[] args)
        {
            var workflow = GetNewWorkFlow();
            workflow.Run();

            Console.WriteLine("Workflow result: " + workflow.Status);

            Console.ReadLine();
        }

        private static WorkFlowState GetNewWorkFlow()
        {
            var workflow = new WorkFlowState();

            workflow.Steps.Add(new Step()
            {
                Name = "Procurement Review",
                StartCondition = StartCondition.Any,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                FailureResult = FailureResult.Ignore,
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
