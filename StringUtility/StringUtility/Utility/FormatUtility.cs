using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringUtility.Common.Text;

namespace StringUtility.Utility
{
    public class FormatUtility : IUtility
    {
        private const string NAME = "Format from csv";

        private string name;

        private string otherInputsText = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool HasOtherInputs { set; get; }

        public string OtherInputsText { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public FormatUtility()
        {
            name = NAME;

            MainName = "Generate Code";

            AdvanceName = "";

            HasOtherInputs = true;
        }

        public string Main(string str, params string[] args)
        {
            var lines = str.Split('\n');

            var builder = new StringBuilder();

            var formatRaw = string.Join(",", args);

            foreach (var line in lines)
            {
                var raw = line.TrimEnd('\r');

                var columns = raw.Split(',');

                var format = formatRaw;

                for (var i = 0; i < columns.Length; i++)
                {
                    if (!columns[i].IsNullOrWhiteSpace() && format.IndexOf("{" + i + "}") >= 0)
                    {
                        format = format.Replace("{" + i + "}", columns[i]);
                    }
                }

                builder.AppendLine(format);
            }

            return builder.ToString();
        }

        public string Advance(string str, params string[] args)
        {
            return string.Empty;
        }
    }
}
