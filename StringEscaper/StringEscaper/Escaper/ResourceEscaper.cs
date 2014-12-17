using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringEscaper.Escaper
{
    public class ResourceEscaper : IEscaper
    {
        private const string NAME = "Resource";

        private string name;

        private bool hasOtherInputs = false;

        private string otherInputsText = "";

        public ResourceEscaper()
        {
            name = NAME;
        }

        public string Escape(string str, params string[] args)
        {
            throw new NotImplementedException();
        }

        public string Unescape(string str)
        {
            return "Hotel";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

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
