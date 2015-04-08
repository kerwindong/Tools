
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("ctranConfig")]
    public class CtranConfig
    {
        [XmlElement("ctranFormatter")]
        public CtranFormatter CtranFormatter { set; get; }
    }

    public class CtranFormatter
    {
        [XmlText]
        public string Value { set; get; }
    }
}
