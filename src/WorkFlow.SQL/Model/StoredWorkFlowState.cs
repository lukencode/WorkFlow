using Nevermore.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlow.Model;

namespace WorkFlow.Sql.Model
{
    public class StoredWorkFlowState : WorkFlowState, IId
    {
        public new string Id => base.Id;
    }
}
