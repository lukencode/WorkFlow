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
        public ObservableCollection<Step> Steps { get; private set; } = new ObservableCollection<Step>();

        public WorkFlowState()
        {
            Id = Guid.NewGuid().ToString();
            Steps.CollectionChanged += StepsChanged;
        }

        public WorkFlowState(IWorkFlowStorage storage) : this()
        {
            this.storage = storage;
        }
        
        private void StepUpdate(object sender, EventArgs e)
        {
            Save();
        }

        private void StepExit(object sender, EventArgs e)
        {
            Save();
            Run();
        }

        public IEnumerable<Step> GetNextSteps(StartCondition startCondition)
        {
            var queue = new Queue<Step>(Steps.Where(x => x.StartCondition == startCondition && x.GetStatus() == Status.None));
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

        private void StepsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Step s in e.NewItems)
                {
                    s.OnExitEvent += StepExit;
                    s.OnUpdateEvent += StepUpdate;
                }
            }
        }

        public void Run()
        {
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

            foreach(var s in nextSteps)
            {
                s.Enter();
            }
        }

        internal void Save() //somehow transactions?
        {
            if (storage == null) return;
            storage.Save(this);
        }
    }
}
