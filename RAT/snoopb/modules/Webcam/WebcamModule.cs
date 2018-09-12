using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.Webcam
{
    public class WebcamModule: Module
    {
        public WebcamModule(int minutesInterval) : base(minutesInterval)
        {
        }

        protected override void ExecuteInternal()
        {
     
        }

        public new static string ModuleId
        {
            get { return "bd57985d-0b8b-4388-ae38-3b000a73868c"; }
        }
    }
}
