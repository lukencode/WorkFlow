using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public abstract class Action
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Note { get; private set; }
        public string User { get; private set; }

        public DateTime? Started { get; private set; }
        public DateTime? Updated { get; private set; }
        public Status Status { get; private set; } = Status.None;
        
        internal event EventHandler OnUpdateEvent;
        internal event EventHandler OnExitEvent;

        public bool CanUpdate => Status == Status.InProgress;
        public bool IsFinished => Status == Status.Success || Status == Status.Failure;

        public Action()
        {
            Id = Guid.NewGuid().ToString();
        }

        internal void Enter()
        {
            Started = DateTime.UtcNow;
            Status = Status.InProgress;
            OnEnter();
        }
        
        public void Update(Status status, string user = null, string note = null, Dictionary<string, object> data = null)
        {
            if (!CanUpdate) throw new ApplicationException("Cannot update a step without Started status");

            Updated = DateTime.UtcNow;
            Status = status;
            User = user;
            Note = note;

            OnUpdate(data);
            //OnUpdateEvent?.Invoke(this, EventArgs.Empty);

            if (IsFinished)
            {
                OnExit();
                OnExitEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnUpdateEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(Dictionary<string, object> data);
        protected abstract void OnExit();
    }
}
