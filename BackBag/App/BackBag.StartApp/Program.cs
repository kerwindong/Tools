
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BackBag.Business.App.Components;
using BackBag.Common.Common;

namespace BackBag.StartApp
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);

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
