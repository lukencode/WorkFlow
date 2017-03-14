using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Model;

namespace WorkFlowExample
{
    public class WaitReadLineStep : Step
    {
        protected override void OnEnter()
        {
            Console.WriteLine($"Entering {Title}: { Id }. Waiting for input...");
            var input = Console.ReadLine();
            Update(Status.Accepted, user: "system", note: input);
        }

        protected override void OnExit()
        {
            Console.WriteLine($"Exiting {Title}: { Id } with state: { Status }");
        }

        protected override void OnUpdate(Dictionary<string, object> data)
        {
            Console.WriteLine($"Updating {Title}: { Id } with state: { Status } and note '{Note}'");
        }
    }
}
