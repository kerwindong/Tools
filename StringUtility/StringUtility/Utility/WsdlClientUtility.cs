using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Schema;

namespace StringUtility
{
    public class WsdlClientUtility : IUtility
    {

        private bool hasOtherInputs = false;
        private string otherInputsText = "";

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

        public WsdlClientUtility()
        {
            Name = "WSDL";

            MainName = "Client Side Code";

            AdvanceName = "Server Side Code";
        }

        private string sourcePath = string.Empty;

        public string Main(string str, params string[] args)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                sourcePath = str;

                var files = GetFiles(str, new List<string>());

                var xsds = default(List<string>);

                var wsdl = string.Empty;

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrWhiteSpace(file))
                        {
                            if (file.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                            {
                                if (xsds == null) xsds = new List<string>();

                                xsds.Add(GetPath(file));
                            }

                            if (string.IsNullOrWhiteSpace(wsdl) && file.EndsWith(".wsdl", StringComparison.OrdinalIgnoreCase))
                            {
                                wsdl = GetPath(file);
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(wsdl) && xsds != null && xsds.Count > 0)
                {
                    return string.Format("wsdl {0} {1}", wsdl, string.Join(" ", xsds.ToArray()));
                }
            }

            return string.Empty;
        }

        public List<string> GetFiles(string path, List<string> files)
        {
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                if (!files.Exists(d => d.Equals(file.FullName)))
                {
                    files.Add(file.FullName);
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                GetFiles(subDirectory.FullName, files);
            }

            return files;
        }

        private string GetPath(string path)
        {
            if (path.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
            {
                return path.Replace(sourcePath, "").TrimStart('\\');
            }

            if (path.EndsWith(".wsdl", StringComparison.OrdinalIgnoreCase))
            {
                return path.Replace(sourcePath, "").TrimStart('\\');
            }

            return string.Empty;
        }

        public string Advance(string str, params string[] args)
        {
            return string.Empty;
        }

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }
    }
}
