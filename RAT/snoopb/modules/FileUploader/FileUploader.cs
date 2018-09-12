using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.FileUploader
{
    public class FileUploader : Module
    {
        public FileUploader(int minutesInterval) : base(minutesInterval)
        {
        }

        protected override void ExecuteInternal()
        {

        }

        public new static string ModuleId
        {
            get { return "81a781ad-d8da-4f3a-9587-e1dc36ba6947"; }
        }
    }
}
