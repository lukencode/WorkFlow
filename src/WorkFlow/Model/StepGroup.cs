using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public class StepGroup : WorkFlowPart
    {
        public Condition Condition { get; set; }
        public List<WorkFlowPart> Steps { get; set; }

        public StepGroup()
        {
            Condition = Condition.AND;
            Steps = new List<WorkFlowPart>();
        }

        public override Status GetStatus()
        {
            var statuses = Steps.Select(x => x.GetStatus());
            var allSameStatus = statuses.All(x => x == statuses.FirstOrDefault());
            if (allSameStatus) return statuses.FirstOrDefault();

            if (Condition == Condition.AND)
            {
                if (statuses.Any(x => x == Status.Rejected)) return Status.Rejected;
                if (statuses.Any(x => x == Status.Started)) return Status.Started;
                if (statuses.Any(x => x == Status.Pending)) return Status.Pending;
            }
            else if (Condition == Condition.OR)
            {
                if (statuses.Any(x => x == Status.Accepted)) return Status.Accepted;
                if (statuses.Any(x => x == Status.Started)) return Status.Started;
            }

            return Status.Pending;
        }

        internal override void Enter()
        {
            foreach (var s in Steps)
            {
                s.Enter();
            }
        }

        internal override IEnumerable<WorkFlowPart> GetAllSteps()
        {
            var stack = new Stack<WorkFlowPart>(new[] { this });
            while (stack.Any())
            {
                var part = stack.Pop();
                yield return part;

                if(part is StepGroup)
                {
                    foreach (var x in (part as StepGroup).Steps) stack.Push(x);
                }
            }
        }

        internal override void AddToWorkFlow(WorkFlowState state)
        {
            base.AddToWorkFlow(state);
            foreach(var step in Steps)
            {
                step.AddToWorkFlow(State);
            }
        }
    }
}
