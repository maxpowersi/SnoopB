using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.Process
{
    public class ProcessModule : Module
    {
        public ProcessModule(int minutesInterval) : base(minutesInterval)
        {
        }

        protected override void ExecuteInternal()
        {
       
        }

        public new static string ModuleId
        {
            get { return "61e543f3-62ca-49a2-a72a-87d3a9adb649"; }
        }
    }
}
