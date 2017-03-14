using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using WorkFlow.Model;

namespace WorkFlow
{
    public class MemoryWorkflowStorage : IWorkFlowStorage
    {
        private static MemoryCache cache = new MemoryCache("WorkFlowStorage");

        public WorkFlowState Load(string id)
        {
            return cache.Get(id) as WorkFlowState;
        }

        public void Save(WorkFlowState state)
        {
            cache.Add(state.Id, state, DateTimeOffset.MaxValue);
        }
    }
}
