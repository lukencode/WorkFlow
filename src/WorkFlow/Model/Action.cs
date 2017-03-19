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

        private EventHandler _onUpdateEvent;
        internal event EventHandler OnUpdateEvent
        {
            add { if (_onUpdateEvent == null) _onUpdateEvent += value; }
            remove { _onUpdateEvent -= value; }
        }

        private event EventHandler _onExitEvent;
        internal event EventHandler OnExitEvent
        {
            add { if (_onExitEvent == null) _onExitEvent += value; }
            remove { _onExitEvent -= value; }
        }

        public bool CanUpdate(Status newStatus) => Status == Status.InProgress || (newStatus == Status.Skipped && CanSkip);
        public bool CanSkip => Status == Status.None;
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
            if (!CanUpdate(status)) throw new ApplicationException("Cannot update a step without Started status");

            Updated = DateTime.UtcNow;
            Status = status;
            User = user;
            Note = note;

            OnUpdate(data);
            //OnUpdateEvent?.Invoke(this, EventArgs.Empty);

            if (IsFinished)
            {
                OnExit();
                _onExitEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _onExitEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(Dictionary<string, object> data);
        protected abstract void OnExit();
    }
}
