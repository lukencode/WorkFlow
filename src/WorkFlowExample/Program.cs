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

            Console.ReadLine();
        }

        private static WorkFlowState GetNewWorkFlow()
        {
            var workflow = new WorkFlowState();

            workflow.Steps.Add(new Step()
            {
                Name = "Procurement Review",
                StartCondition = StartCondition.Success,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                Actions = new ObservableCollection<WorkFlow.Model.Action>()
                {
                    new WaitReadLineStep() { Name = "Procurement Approval" }
                }
            });
            
            workflow.Steps.Add(new Step()
            {
                Name = "Financial Review",
                StartCondition = StartCondition.Success,
                StartTrigger = StartTrigger.StartAfterPrevious,
                SuccessCondition = SuccessCondition.All,
                Actions = new ObservableCollection<WorkFlow.Model.Action>()
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
                Actions = new ObservableCollection<WorkFlow.Model.Action>()
                {
                    new WaitReadLineStep() { Name = "Procurement Approval" }
                }
            });


            return workflow;
        }
    }
}
