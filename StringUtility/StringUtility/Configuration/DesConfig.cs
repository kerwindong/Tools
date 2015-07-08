
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("desConfig")]
    public class DesConfig
    {
        [XmlElement("desPolicy")]
        public DesPolicy DesPolicy { set; get; }
    }

    public class DesPolicy
    {
        [XmlAttribute("key")]
        public string Key { set; get; }
    }
}
