using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow.Model
{
    public class StepGroup
    {
        public Condition Condition { get; set; }
        public List<Step> Actions { get; set; }
    }
}
