using System;
using System.Diagnostics;
using System.Windows.Forms;

using BackBag.Business.App.Components;
using BackBag.Common.Common;

namespace BackBag.Stormtrooper
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            BackBagComponent.Instance.InitRoot();

            var rootApp = BackBagComponent.Instance.BackBag.InstalledApps.Find(d => d.Name.AreEqual("Stormtrooper"));

            var startInfo = new ProcessStartInfo(CommonEnvironment.BaseDirectory + BackBagComponent.APPHOST_LOCAL + rootApp.Name.ToLocal() + rootApp.StartApp.ToLocal());

            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            startInfo.UseShellExecute = false;

            using (var process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.WaitForExit();
                }
            }
        }
    }
}
