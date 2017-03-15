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
        //public List<Step> Steps { get; private set; }

        public List<WorkFlowPart> Steps { get; private set; }

        public WorkFlowState()
        {
            Id = Guid.NewGuid().ToString();
            Steps = new List<WorkFlowPart>();
        }

        public WorkFlowState(IWorkFlowStorage storage) : this()
        {
            this.storage = storage;
        }

        public WorkFlowState AddStep(WorkFlowPart step)
        {
            step.AddToWorkFlow(this);
            Steps.Add(step);
            return this;
        }

        public WorkFlowPart GetNextStep()
        {
            this is broke
            return Steps.Where(x => x.GetStatus() == Status.Pending).FirstOrDefault();
        }

        public List<WorkFlowPart> GetStartedSteps()
        {
            var started = Steps.Where(x => x.GetStatus() == Status.Started);
            return started.ToList();
        }

        public WorkFlowPart GetStep(string id)
        {
            return Steps.SelectMany(x => x.GetAllSteps()).FirstOrDefault(x => x.Id == id);
        }

        public void Run()
        {
            var next = GetNextStep();
            if (next != null)
            {
                next.Enter();
                Save();
            }
        }

        internal void Save() //somehow transactions?
        {
            if (storage == null) return;
            storage.Save(this);
        }
    }
}
