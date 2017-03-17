using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Model;

namespace WorkFlowExample
{
    public class WriteLineStep : WorkFlow.Model.Action
    {
        protected override void OnEnter()
        {
            Console.WriteLine($"Entering {Name}: { Id }");
            Update(Status.Success, "system");
        }

        protected override void OnExit()
        {
            Console.WriteLine($"Exiting {Name}: { Id } with state: { Status }");
        }

        protected override void OnUpdate(Dictionary<string, object> data)
        {
            Console.WriteLine($"Updating {Name}: { Id } with state: { Status }");
        }
    }
}
