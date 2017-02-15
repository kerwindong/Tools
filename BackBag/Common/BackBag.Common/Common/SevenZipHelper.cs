
using System;
using System.Diagnostics;

using BackBag.Common.Log;

namespace BackBag.Common.Common
{
    public static class SevenZipHelper
    {
        public static void Decompress(string inFile, string outFile)
        {
            var processInfo = new ProcessStartInfo();

            processInfo.WindowStyle = ProcessWindowStyle.Hidden;

            processInfo.FileName = CommonEnvironment.BaseDirectory + "\\7z.exe";

            processInfo.Arguments = string.Format(" x \"{0}\" -o\"{1}\" -aoa ", inFile, outFile);

            using (var process = Process.Start(processInfo))
            {
                if (process != null)
                {

                    process.ErrorDataReceived += process_ErrorDataReceived;
                    process.WaitForExit();
                }
            }
        }

        public static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            FileLogger.Instance.Log(new Exception(e.Data));
        }
    }
}
