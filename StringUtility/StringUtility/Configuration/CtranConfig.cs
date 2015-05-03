
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("ctranConfig")]
    public class CtranConfig
    {
        [XmlElement("ctranFormatter")]
        public FormatterConfig CtranFormatter { set; get; }

        [XmlElement("excelFormatter")]
        public FormatterConfig ExcelFormatter { set; get; }

        [XmlElement("propertyFormatter")]
        public FormatterConfig PropertyFormatter { set; get; }
    }

    public class FormatterConfig
    {
        [XmlText]
        public string Value { set; get; }
    }
}
