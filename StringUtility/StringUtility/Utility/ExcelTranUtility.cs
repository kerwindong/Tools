
using System.Collections.Generic;
using System.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class ExcelTranUtility : IUtility
    {
        private const string CTRAN = "Excel Transaction Coder";

        private bool hasOtherInputs = false;

        private string otherInputsText = "";

        private string name = "";

        private Dictionary<string, Dictionary<string, string>> languageDictionary;

        private Dictionary<int, string> languageIndexDictionary;

        private string excelFormat = string.Empty;

        public ExcelTranUtility()
        {
            name = CTRAN;

            MainName = "Generate Code";

            AdvanceName = "";

            hasOtherInputs = false;

            otherInputsText = string.Empty;

            excelFormat = ConfigManager.Get().CtranConfig.ExcelFormatter.Value;
        }

        public string Main(string str, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            languageDictionary = new Dictionary<string, Dictionary<string, string>>();

            languageIndexDictionary = new Dictionary<int, string>();

            var lines = str.Split('\n');

            if (lines.Length > 0)
            {
                var firstLine = lines[0];

                if (!string.IsNullOrWhiteSpace(firstLine))
                {
                    var firstLineColumns = firstLine.Split(',');

                    if (firstLineColumns.Length > 1)
                    {
                        for (var i = 1; i < firstLineColumns.Length; i++)
                        {
                            var languageName = firstLineColumns[i].Trim();

                            if (!string.IsNullOrWhiteSpace(languageName) && 
                                !languageIndexDictionary.ContainsKey(i) &&
                                !languageDictionary.ContainsKey(languageName))
                            {
                                languageIndexDictionary.Add(i, languageName);

                                languageDictionary.Add(languageName, new Dictionary<string, string>());
                            }
                        }
                    }

                    for (var i = 1; i < lines.Length; i++)
                    {
                        var line = lines[i];

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var lineColumns = line.Split(',');

                            if (lineColumns.Length > 1)
                            {
                                var keyName = lineColumns[0].Trim();

                                if (!string.IsNullOrWhiteSpace(keyName))
                                {
                                    for (var j = 1; j < lineColumns.Length; j++)
                                    {
                                        var value = lineColumns[j].Trim();

                                        if (!string.IsNullOrWhiteSpace(value) && languageIndexDictionary.ContainsKey(j))
                                        {
                                            var languageName = languageIndexDictionary[j];

                                            if (!string.IsNullOrWhiteSpace(languageName) && 
                                                languageDictionary.ContainsKey(languageName))
                                            {
                                                var keyValueDictionary = languageDictionary[languageName];

                                                if (keyValueDictionary == null)
                                                {
                                                    keyValueDictionary = new Dictionary<string, string>();
                                                }

                                                if (!keyValueDictionary.ContainsKey(keyName))
                                                {
                                                    keyValueDictionary.Add(keyName, value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var builder = new StringBuilder();

            if (languageDictionary.Count > 0)
            {
                foreach (var languageName in languageDictionary.Keys)
                {
                    builder.AppendLine();

                    builder.AppendLine(string.Format("{0}------------------------------", languageName));

                    var keyValues = languageDictionary[languageName];

                    foreach (var keyName in keyValues.Keys)
                    {
                        var value = keyValues[keyName];

                        builder.Append(string.Format(excelFormat, keyName, value));
                    }

                    builder.AppendLine();
                }
                
            }

            return builder.ToString();
        }

        public string Advance(string str)
        {
            return "not implemented";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public bool HasOtherInputs
        {
            get { return hasOtherInputs; }
            set { hasOtherInputs = value; }
        }

        public string OtherInputsText
        {
            get { return otherInputsText; }
            set { otherInputsText = value; }
        }
    }
}
