using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public class WorkFlowState
    {
        private readonly IWorkFlowStorage storage;

        public string Id { get; private set; }
        public List<Step> Steps { get; private set; } = new List<Step>();

        public DateTime? Started { get; private set; }
        public DateTime? Updated { get; private set; }
        public DateTime? Completed { get; private set; }
        public Status Status { get; private set; } = Status.None;

        public WorkFlowState()
        {
            Id = Guid.NewGuid().ToString();
        }

        public WorkFlowState(IWorkFlowStorage storage) : this()
        {
            this.storage = storage;
        }

        private void StepUpdate(object sender, EventArgs e)
        {
            Updated = DateTime.UtcNow;
            Save();
        }

        private void StepExit(object sender, EventArgs e)
        {
            Updated = DateTime.Now;
            Run();
        }

        public IEnumerable<Step> GetNextSteps(StartCondition startCondition)
        {
            var validSteps = from s in Steps
                             where (s.StartCondition == StartCondition.Any || s.StartCondition == startCondition)
                             && s.GetStatus() == Status.None
                             select s;

            var queue = new Queue<Step>(validSteps);

            if (!queue.Any()) yield break;

            yield return queue.Dequeue();

            while(queue.Any())
            {
                var next = queue.Dequeue();
                if (next.StartTrigger != StartTrigger.StartParallelPrevious) yield break;
                yield return next;
            }
        }

        public List<Step> GetStartedSteps() => Steps.Where(x => x.GetStatus() == Status.InProgress).ToList();

        public Step GetStep(string id) => Steps.FirstOrDefault(x => x.Id == id);

        public Action GetAction(string id) => Steps.SelectMany(x => x.Actions).FirstOrDefault(x => x.Id == id);

        private bool IsCompletedStatus(Status status) => Status == Status.Failure || Status == Status.Success;

        public void Run()
        {
            if (!Started.HasValue)
            {
                Started = DateTime.UtcNow;
                Status = Status.InProgress;
                Save();
            }

            //todo maybe use step index instead of dates
            var previousStep = (from s in Steps
                                let status = s.GetStatus()
                                let latestAction = s.Actions.Where(x => x.Updated.HasValue).OrderByDescending(x => x.Updated).FirstOrDefault()
                                where latestAction != null
                                && status != Status.Skipped
                                orderby latestAction.Updated descending
                                select new { Step = s, Status = status }).FirstOrDefault();

            if (previousStep != null && previousStep.Status == Status.InProgress) return;

            var previousStepStatus = previousStep?.Status ?? Status.Success; //default to success    
            var nextStepStartCondition = previousStepStatus == Status.Success ? StartCondition.Success : StartCondition.Failure;
            var nextSteps = GetNextSteps(nextStepStartCondition);

            foreach (var s in nextSteps)
            {
                //ensure events hooked up
                s.OnExitEvent += StepExit;
                s.OnUpdateEvent += StepUpdate;

                s.Enter();
            }

            Status = GetStatus();

            if(IsCompletedStatus(Status))
            {
                if(Status == Status.Failure)
                    SkipRemainingSteps();

                Completed = DateTime.UtcNow;
            }
                        
            Save();
        }

        private void SkipRemainingSteps()
        {
            var remainingSteps = Steps.Where(x => x.GetStatus() == Status.None);
            foreach (var s in remainingSteps)
            {
                s.SkipStep();
            }
        }

        private Status GetStatus()
        {
            var stepStatuses = Steps.Select(x => new { Step = x, Status = x.GetStatus(), FailureResult = x.FailureResult });

            if (stepStatuses.Any(x => x.Status == Status.InProgress)) return Status.InProgress;
            if (stepStatuses.All(x => x.Status == Status.None)) return Status.None;
            if (stepStatuses.All(x => x.Status == Status.Skipped)) return Status.Skipped;

            var withoutIgnored = stepStatuses.Where(x => x.Status != Status.Skipped && x.FailureResult == FailureResult.FailWorkflow);

            if (withoutIgnored.Any(x => x.Status == Status.Failure)) return Status.Failure;
            if (withoutIgnored.Any(x => x.Status == Status.None || x.Status == Status.InProgress)) return Status.InProgress;
            if (withoutIgnored.All(x => x.Status == Status.Success)) return Status.Success;

            throw new ApplicationException("Invalid workflow status");
        }

        internal void Save() //somehow transactions?
        {
            if (storage == null) return;
            storage.Save(this);
        }
    }
}
