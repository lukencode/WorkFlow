using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public class Step
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public StartCondition StartCondition { get; set; } = StartCondition.Success;
        public StartTrigger StartTrigger { get; set; } = StartTrigger.StartAfterPrevious;
        public SuccessCondition SuccessCondition { get; set; } = SuccessCondition.All;
        public ObservableCollection<Action> Actions { get; set; }

        internal event EventHandler OnUpdateEvent;
        internal event EventHandler OnExitEvent;

        public Step()
        {
            Actions = new ObservableCollection<Action>();
            Actions.CollectionChanged += ActionsChanged;
            Id = Guid.NewGuid().ToString();
        }

        private void ActionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Action a in e.NewItems)
                {
                    a.OnExitEvent += ActionExit;
                    a.OnUpdateEvent += ActionUpdate;
                }
            }
        }
        
        private void ActionUpdate(object sender, EventArgs e)
        {
            OnUpdateEvent?.Invoke(this, EventArgs.Empty);
        }

        private void ActionExit(object sender, EventArgs e)
        {
            var actionsRemaining = Actions.Any(x => !x.IsFinished && x.Status != Status.Skipped);
            if(!actionsRemaining)
                OnExitEvent?.Invoke(this, EventArgs.Empty);
        }

        public Status GetStatus()
        {
            if (Actions.All(x => x.Status == Status.None)) return Status.None;
            if (Actions.All(x => x.Status == Status.Skipped)) return Status.Skipped;

            var actionsWithoutSkipped = Actions.Where(x => x.Status != Status.Skipped);

            if (SuccessCondition == SuccessCondition.All)
            {
                if (actionsWithoutSkipped.Any(x => x.Status == Status.Failure)) return Status.Failure;
                if (actionsWithoutSkipped.Any(x => x.Status == Status.InProgress)) return Status.InProgress;
                if (actionsWithoutSkipped.All(x => x.Status == Status.Success)) return Status.Success;
            }
            else if(SuccessCondition == SuccessCondition.Any)
            {
                if (actionsWithoutSkipped.Any(x => x.Status == Status.Success)) return Status.Success;
                if (actionsWithoutSkipped.Any(x => x.Status == Status.InProgress)) return Status.InProgress;
                if (actionsWithoutSkipped.All(x => x.Status == Status.Failure)) return Status.Failure;
            }

            return Status.None;
        }

        internal void Enter()
        {
            var stepStatus = GetStatus();
            if (stepStatus != Status.None && stepStatus != Status.InProgress)
                throw new ApplicationException($"Cannot enter a completed step. Step status: {stepStatus}");

            foreach (var a in Actions.Where(x => x.Status == Status.None))
            {
                //maybe in the future this should use similar start status to above
                a.Enter();
            }

            OnUpdateEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
