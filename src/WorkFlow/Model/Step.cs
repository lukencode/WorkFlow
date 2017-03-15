using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public abstract class Step : WorkFlowPart
    {
        public string Title { get; set; }
        public string Note { get; private set; }
        public string User { get; private set; }

        public DateTime? Started { get; private set; }
        public DateTime? Updated { get; private set; }
        public Status Status { get; private set; } = Status.Pending;

        public bool CanUpdate => Status == Status.Started;
        private List<WorkFlowPart> ListOfThis => new List<WorkFlowPart> { this };


        public override Status GetStatus()
        {
            return Status;
        }

        internal override void Enter()
        {
            Started = DateTime.UtcNow;
            Status = Status.Started;
            OnEnter();
        }


        internal override IEnumerable<WorkFlowPart> GetAllSteps()
        {
            return ListOfThis;
        }

        internal override List<WorkFlowPart> GetNextSteps()
        {
            if (Status == Status.Pending) return ListOfThis;
            return new List<WorkFlowPart>();
        }

        internal override List<WorkFlowPart> GetStartedSteps()
        {
            if (Status == Status.Started) return ListOfThis;
            return new List<WorkFlowPart>();
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
        }

        protected abstract void OnEnter();
        protected abstract void OnUpdate(Dictionary<string, object> data);
        protected abstract void OnExit();

    }
}
