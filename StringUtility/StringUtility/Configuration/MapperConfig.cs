
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("mapperConfig")]
    public class MapperConfig
    {
        [XmlElement("mapperFormatter")]
        public MapperFormatter MapperFormatter { set; get; }
    }

    public class MapperFormatter
    {
        [XmlText]
        public string Value { set; get; }
    }
}
