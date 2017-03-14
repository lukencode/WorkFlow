using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public abstract class Step
    {
        internal WorkFlowState State;

        public string Id { get; private set; }
        public string Title { get; set; }
        public string Note { get; private set; }
        public string User { get; private set; }

        public DateTime? Started { get; private set; }
        public DateTime? Updated { get; private set; }
        public Status Status { get; private set; }

        public bool CanUpdate => Status == Status.Started;

        internal void AddToWorkFlow (WorkFlowState state)
        {
            Id = Guid.NewGuid().ToString();
            State = state;
        }

        internal void Enter()
        {
            Started = DateTime.UtcNow;
            Status = Status.Started;
            OnEnter();

            State.Save();
        }

        public void Update(Status status, string user = null, string note = null, Dictionary<string, object> data = null)
        {
            if (!CanUpdate) throw new ApplicationException("Cannot update a step without Started status");

            Updated = DateTime.UtcNow;
            Status = status;
            User = user;
            Note = note;

            OnUpdate(data);

            if (status == Status.Accepted || status == Status.Rejected)
            {
                OnExit();
                State.Run();
            }

            State.Save();
        }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(Dictionary<string, object> data);
        protected abstract void OnExit();
    }
}
