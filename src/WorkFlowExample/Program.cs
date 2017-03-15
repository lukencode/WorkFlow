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
            workflow.Run();

            Console.ReadLine();
        }

        private static WorkFlowState GetNewWorkFlow()
        {
            var workflow =
                new WorkFlowState()
                        .AddStep(new StepGroup
                        {
                            Steps = new List<WorkFlowPart>()
                            {
                                new WaitReadLineStep { Title = "Step 1" },
                                new StepGroup
                                {
                                    Condition = Condition.AND,
                                    Steps = new List<WorkFlowPart>()
                                    {
                                        new WaitReadLineStep { Title = "Step 2.1" },
                                        new WaitReadLineStep { Title = "Step 2.2" },
                                        new WaitReadLineStep { Title = "Step 2.3" }
                                    }
                                },
                                new WaitReadLineStep
                                {
                                    Title = "Step 3"
                                }
                            }
                        });

            return workflow;
        }
    }
}
