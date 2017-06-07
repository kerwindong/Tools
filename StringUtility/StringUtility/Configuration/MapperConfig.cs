
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("mapperConfig")]
    public class MapperConfig
    {
        [XmlElement("mapperFormatter")]
        public List<MapperFormatter> MapperFormatter { set; get; }
    }

    public class MapperFormatter
    {
        [XmlText]
        public string Value { set; get; }

        [XmlAttribute("type")]
        public string Type { set; get; }
    }
}
