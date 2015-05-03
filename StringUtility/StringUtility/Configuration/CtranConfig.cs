
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    [XmlRoot("ctranConfig")]
    public class CtranConfig
    {
        [XmlElement("ctranFormatter")]
        public CtranFormatter CtranFormatter { set; get; }

        [XmlElement("excelFormatter")]
        public ExcelFormatter ExcelFormatter { set; get; }
    }

    public class CtranFormatter
    {
        [XmlText]
        public string Value { set; get; }
    }

    public class ExcelFormatter
    {
        [XmlText]
        public string Value { set; get; }
    }
}
