using System;
using System.Collections.Generic;
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
            storage.Save(workflow);

            Console.ReadLine();

            var savedWorkflow = storage.Load(workflow.Id);
            savedWorkflow.Run();

            //var next = savedWorkflow.GetNextSteps();

            //foreach (var n in savedWorkflow.GetNextSteps())
            //{
            //    n.Update(Status.Accepted, "system");
            //}

            Console.ReadLine();
        }

        private static WorkFlowState GetNewWorkFlow()
        {
            var workflow =
                new WorkFlowState(storage)
                        .AddStep(new WriteLineStep
                        {
                            Title = "Step 1"
                        })
                        .AddStep(new WaitReadLineStep
                        {
                            Title = "Waiting Step 2"
                        })
                        .AddStep(new WriteLineStep
                        {
                            Title = "Step 3"
                        });

            return workflow;
        }
    }
}
