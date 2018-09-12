using System;
using System.Collections.Generic;
using System.Text;

namespace SnoopB.Modules.FileExplorer
{
    public class FileExplorerModule: Module
    {
        public FileExplorerModule(int minutesInterval) : base(minutesInterval)
        {
        }

        protected override void ExecuteInternal()
        {
        
        }

        public new static string ModuleId
        {
            get { return "f9b16de9-eea6-4d56-aad3-fb035041f4ab"; }
        }
    }
}