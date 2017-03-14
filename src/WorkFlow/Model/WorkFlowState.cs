using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public class WorkFlowState
    {
        private readonly IWorkFlowStorage storage;

        public string Id { get; private set; }
        public List<Step> Steps { get; private set; }

        public WorkFlowState()
        {
            Id = Guid.NewGuid().ToString();
            Steps = new List<Step>();
        }

        public WorkFlowState(IWorkFlowStorage storage) : this()
        {
            storage = this.storage;
        }

        public WorkFlowState AddStep(Step step)
        {
            step.AddToWorkFlow(this);
            Steps.Add(step);
            return this;
        }

        public List<Step> GetNextSteps()
        {
            var next = Steps.Where(x => x.Status == Status.Pending).Take(1); //todo this would change with grouping
            return next.ToList();
        }

        public List<Step> GetStartedSteps()
        {
            var started = Steps.Where(x => x.Status == Status.Started);
            return started.ToList();
        }

        public void Run()
        {
            var next = GetNextSteps();
            foreach (var n in next)
            {
                n.Enter();
            }
        }

        internal void Save() //somehow transactions?
        {
            if (storage == null) return;
            storage.Save(this);
        }
    }
}
