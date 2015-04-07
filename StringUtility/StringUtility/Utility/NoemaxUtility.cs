using System;
using System.IO;
using System.Xml;

using Noemax.FastInfoset;

namespace StringUtility
{
    public class NoemaxUtility : IUtility
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

        // Methods
        public NoemaxUtility()
        {
            this.Name = "Noemax";

            MainName = "Decode";

            AdvanceName = "";
        }

        public string Main(string str, params string[] args)
        {
            string str2 = string.Empty;
            if (!string.IsNullOrWhiteSpace(str))
            {
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(str)))
                {
                    using (XmlReader reader = new XmlFastInfosetReader(stream, FastInfosetCompression.GZip))
                    {
                        reader.MoveToElement();
                        bool flag = reader.Read();
                        while (flag && (reader.NodeType != XmlNodeType.Element))
                        {
                            flag = reader.Read();
                        }
                        if (flag)
                        {
                            str2 = reader.ReadOuterXml();
                        }
                    }
                }
            }
            return str2;
        }

        public string Advance(string str)
        {
            return "Not Implemented";
        }

        // Properties
        public string Name { get; set; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }
    }


}
