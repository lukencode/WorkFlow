using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public abstract class WorkFlowPart
    {
        internal WorkFlowState State;

        public string Id { get; private set; }

        internal virtual void AddToWorkFlow(WorkFlowState state)
        {
            Id = Guid.NewGuid().ToString();
            State = state;
        }
    }
}
