using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SnoopBCleaner
{
    public class Cleaner
    {
        const string NameKeyMain = "WinProcessSecurity";
        const string Extension = ".exe";
 
        public void Clean()
        {
            var pathMain = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), NameKeyMain), NameKeyMain + Extension);
            var noEvidencia = true;

            //Limpiar registro
            var rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk.GetValue(NameKeyMain) != null)
            {
                rk.DeleteValue(NameKeyMain);
                noEvidencia = false;
            }

            //Limpiar archivos
            if (Directory.Exists(Path.GetDirectoryName(pathMain)))
            {
                noEvidencia = false;

                var procesos = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(pathMain));
                if (procesos.Length != 0)
                    procesos[0].Kill();

                //Borramos
                foreach (var f in Directory.GetFiles(Path.GetDirectoryName(pathMain)))
                    File.Delete(f);

                Directory.Delete(Path.GetDirectoryName(pathMain));
            }

            //Mensaje final
            var msj = noEvidencia ? Phrases.NoSnoopB : Phrases.CleanDone;
            MessageBox.Show(msj, Phrases.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
