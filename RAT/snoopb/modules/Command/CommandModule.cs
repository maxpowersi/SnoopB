using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.Command
{
    public class CommandModule : Module
    {
        public CommandModule(int minutesInterval) : base(minutesInterval)
        {

        }

        protected override void ExecuteInternal()
        {

        }

        public new static  string ModuleId
        {
            get { return "6017eb5b-d207-4793-8746-e6ed8b827b2f"; }
        }
    }
}
