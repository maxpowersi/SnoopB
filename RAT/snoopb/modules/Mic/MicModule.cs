using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.Mic
{
    public class MicModule : Module
    {
        public MicModule(int minutesInterval) : base(minutesInterval)
        {

        }

        protected override void ExecuteInternal()
        {

        }

        public new static string ModuleId
        {
            get { return "b0e46d9f-a589-40f3-beea-8abe16a97309"; }
        }
    }
}
