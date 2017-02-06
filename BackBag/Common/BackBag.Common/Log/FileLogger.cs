using System;

using System.IO;
using System.Text;
using BackBag.Common.Common;

namespace BackBag.Common.Log
{
    public sealed class FileLogger 
    {
        private const string LOG_FILE_NAME_FORMATTER = "yyyyMMddHH";
        private const string LOG_FILE_EXT_NAME = ".log";
        private const string LOG_DIRECTORY = "\\Log\\";

        private const string LOG_SPLIT = "--------------------------------------------------------------------------------------------------------------------------------------------------";
        private const string LOG_TITLE = "Title:";
        private const string LOG_EXCEPTION = "Exception:";

        private readonly static object locker = new object();

        private string logPath;

        private byte[] newLine;

        private FileLogger()
        {
            logPath = string.Concat(CommonEnvironment.BaseDirectory, LOG_DIRECTORY);

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            newLine = Encoding.UTF8.GetBytes(Environment.NewLine);
        }

        private void Log(CommonException exception)
        {
            lock (locker)
            {
                if (exception != null)
                {
                    var now = DateTime.Now;

                    var logFile = string.Concat(logPath, now.ToString(LOG_FILE_NAME_FORMATTER), LOG_FILE_EXT_NAME);

                    using (var stream = new FileStream(logFile, FileMode.OpenOrCreate))
                    {
                        if (stream.Length > 0)
                        {
                            stream.Position = stream.Length - 1;
                        }

                        WriteStream(stream, LOG_SPLIT);

                        if (!string.IsNullOrWhiteSpace(exception.Title))
                        {
                            WriteStream(stream, LOG_TITLE);
                            WriteStream(stream, exception.Title);
                        }

                        if (exception.Exception != null)
                        {
                            WriteStream(stream, LOG_EXCEPTION);
                            WriteStream(stream, exception.Exception.ToString());
                        }

                        WriteLine(stream);
                        WriteLine(stream);

                        stream.Close();

                        System.Threading.Thread.Sleep(200);
                    }
                }
            }
        }

        private void WriteStream(FileStream fileStream, string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var bytes = Encoding.UTF8.GetBytes(str);

                if (bytes.Length > 0)
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                    WriteLine(fileStream);
                }
            }
        }

        private void WriteLine(FileStream fileStream)
        {
            fileStream.Write(newLine, 0, newLine.Length);
        }

        public void Log(string title, Exception exception)
        {
            Log(new CommonException(title, exception, null));
        }

        public void Log(Exception exception)
        {
            Log(new CommonException(string.Empty, exception, null));
        }

        #region Singleton

        public static FileLogger Instance { get { return InternalFileLogger.Instance; } }

        private class InternalFileLogger
        {
            // Tell C# compiler not to mark type as beforefieldinit
            static InternalFileLogger()
            {
            }

            internal static readonly FileLogger Instance = new FileLogger();
        }

        #endregion
    }
}
