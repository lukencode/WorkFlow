using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Model;

namespace WorkFlow
{
    public interface IWorkFlowStorage
    {
        void Save(WorkFlowState state);
        WorkFlowState Load(string id);
    }
}
