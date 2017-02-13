﻿using System;

namespace BackBag.Common.Common
{
    public static class StringExtension
    {
        public static bool AreEqual(this string source, string target)
        {
            return string.Compare(source, target, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string ToLocal(this string file)
        {
            return "\\" + file.TrimStart('/');
        }
    }
}
