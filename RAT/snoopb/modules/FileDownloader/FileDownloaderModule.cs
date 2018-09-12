using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.FileDownloader
{
    public class FileDownloaderModule: Module
    {
        public FileDownloaderModule(int minutesInterval) : base(minutesInterval)
        {

        }

        protected override void ExecuteInternal()
        {

        }

        public new static string ModuleId
        {
            get { return "f7e7fb09-c5fd-40bf-9b29-b045cad2004b"; }
        }
    }
}
