using System;
using System.Windows.Forms;

using BackBag.Business.App.Components;

namespace BackBag.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Main());
        }
    }
}
