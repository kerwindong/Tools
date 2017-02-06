using System;

namespace BackBag.Common.Common
{
    public class CommonEnvironment
    {
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
    }
}
