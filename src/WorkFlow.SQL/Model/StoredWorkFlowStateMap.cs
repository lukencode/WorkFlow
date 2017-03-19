using Nevermore.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Sql.Model
{
    public class StoredWorkFlowStateMap : DocumentMap<StoredWorkFlowState>
    {
        public StoredWorkFlowStateMap()
        {
            Column(x => x.Completed);
            Column(x => x.Started);
            Column(x => x.Status);
        }
    }
}
