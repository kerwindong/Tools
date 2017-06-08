using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using StringUtility.Common.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class JavaEnumMapperProperty
    {
        public string TypeName { set; get; }

        public string PropertyName { set; get; }
    }

    public class JavaEnumMapper : IUtility
    {
        private static readonly char[] IDENTIFIER_BREAKER = new char[] { '\n' };
        private static readonly char[] IDENTIFIER_WRAPPER_BEGIN = new char[] { '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_END = new char[] { '}' };
        private static readonly char[] ENUM_WRAPPER_END = new char[] { ')' };
        private static readonly char[] SPLIT = new char[] { ';' };
        private static readonly char[] SPACE = new char[] { ' ' };
        private static readonly char[] NEWLINE = new char[] { '\n' };

        private static readonly char[] ENUM_IDENTIFIER = new char[] { 'e', 'n', 'u', 'm' };
        private static readonly char[] PROPERTY_IDENTIFIER_SCOPE = new char[] { '\n' };

        private static readonly char[] PROTOCONTRACT_IDENTIFIER = new char[] { '[', 'P', 'r', 'o', 't', 'o', 'C', 'o', 'n', 't', 'r', 'a', 'c', 't', ']' };

        private const string TAB = "    ";
        private const string MAPPER = "Java Enum With Integer";
        private const string TARGET_CLASS_NAME = "";

        private const string PROPERTY_FORMAT = "        {1}({0}){2}";

        private string name;

        private string data = string.Empty;

        private string dataOut = string.Empty;

        private StringBuilder builder = default(StringBuilder);
        private int length = 0;
        private int buildIndex = 0;
        private int lookForwardIndex = 0;
        private int memberIndex = 1;
        private string className = string.Empty;
        private List<MapperProperty> propertyNames = default(List<MapperProperty>);

        private string mapperFormatterValue = string.Empty;

        private bool hasOtherInputs = false;
        private string otherInputsText = "";

        private static readonly Dictionary<string, string> typeMapper = new Dictionary<string, string>()
        {
            { "Long", "long" },

            { "Integer", "int" },

            { "Double", "double" }
        };

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

        public JavaEnumMapper()
        {
            name = MAPPER;

            MainName = "Generate Code";

            AdvanceName = "";

            mapperFormatterValue = ConfigManager.Get().MapperConfig.MapperFormatter.Find(d => d.Type == "JavaEnum").Value;

            hasOtherInputs = false;

            otherInputsText = TARGET_CLASS_NAME;
        }

        public string Main(string str, params string[] args)
        {
            data = string.Empty;
            dataOut = string.Empty;

            data = str;

            length = data.Length;

            buildIndex = 0;
            lookForwardIndex = 0;
            memberIndex = 1;
            className = string.Empty;
            propertyNames = new List<MapperProperty>();

            builder = new StringBuilder();

            while (true)
            {
                if (lookForwardIndex >= length)
                {
                    break;
                }

                lookForwardIndex = GetClassName(lookForwardIndex);

                if (!className.IsNullOrWhiteSpace())
                {
                    break;
                }

                lookForwardIndex++;
            }

            while (true)
            {
                if (lookForwardIndex >= length)
                {
                    break;
                }

                lookForwardIndex = GetPropertyName(lookForwardIndex);

                lookForwardIndex++;
            }

            var classBuilder = new StringBuilder();

            if (!propertyNames.Any())
            {
                propertyNames = new List<MapperProperty>()
                {
                    new MapperProperty()
                    {
                        PropertyName = "ENUM_KEY",

                        TypeName = "0"
                    }
                };
            }

            for (var i = 0; i < propertyNames.Count; i++)
            {
                var propertyName = propertyNames[i];

                var isLast = i + 1 == propertyNames.Count;

                classBuilder.AppendLine(string.Format(PROPERTY_FORMAT, propertyName.TypeName, propertyName.PropertyName.ToUpper(), isLast ? ";" : ","));

                if (!isLast)
                {
                    classBuilder.AppendLine();
                }
            }

            if (className.IsNullOrWhiteSpace())
            {
                className = "EnumName";
            }

            var classRaw = string.Format(mapperFormatterValue, className, classBuilder.ToString());

            builder.AppendLine(classRaw);

            builder.AppendLine();

            // Out 

            dataOut = builder.ToString();

            return dataOut;
        }

        public string Advance(string str, params string[] args)
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

        private int GetClassName(int startIndex)
        {
            if (length >= startIndex + ENUM_IDENTIFIER.Length)
            {
                int classNameStart = LookForward(startIndex, ENUM_IDENTIFIER, ENUM_IDENTIFIER.Length, SPACE);

                if (classNameStart >= 0)
                {
                    int wrapperStart = LookForward(startIndex, NEWLINE, NEWLINE.Length, IDENTIFIER_WRAPPER_BEGIN);

                    if (wrapperStart > classNameStart)
                    {
                        var classNameFull = data.Substring(classNameStart, wrapperStart - classNameStart - 1).Trim();

                        var classNameFulls = classNameFull.Replace(':', ' ').Split(' ');

                        if (classNameFulls.Length >= 0)
                        {
                            className = classNameFulls[0].Trim();
                        }

                        startIndex = wrapperStart;
                    }
                }
            }

            return startIndex;
        }

        private int GetPropertyName(int startIndex)
        {
            if (length >= startIndex + NEWLINE.Length)
            {
                int propertyNameStart = LookForward(startIndex, NEWLINE, NEWLINE.Length, SPACE);

                if (propertyNameStart >= 0)
                {
                    int wrapperStart = LookForward(startIndex, ENUM_WRAPPER_END, 1, new[] { ';', ',' });

                    if (wrapperStart > propertyNameStart)
                    {
                        var propertyNameFull = data.Substring(propertyNameStart, wrapperStart - propertyNameStart - 1).Trim();

                        var propertyNameFulls = propertyNameFull.Split('(');

                        if (propertyNameFulls.Length >= 1)
                        {
                            propertyNames.Add(new MapperProperty()
                            {
                                PropertyName = propertyNameFulls[0].Trim(),

                                TypeName = propertyNameFulls[1].TrimEnd(';', ')')
                            });
                        }

                        startIndex = wrapperStart;
                    }
                }
            }

            return startIndex;
        }

        private void Append(int startIndex, int length)
        {
            for (int i = 0; i < length; i++)
            {
                builder.Append(data[startIndex + i]);
            }
            builder.Append(Environment.NewLine);
        }

        private void AppendProtoContract()
        {
            builder.Append(TAB);
            for (int i = 0; i < PROTOCONTRACT_IDENTIFIER.Length; i++)
            {
                builder.Append(PROTOCONTRACT_IDENTIFIER[i]);
            }
        }

        private int LookBackward(int index, char[] identifier, int offset, char[] breakIdentifier)
        {
            int currentIndex = index;
            while (currentIndex >= offset)
            {
                if (breakIdentifier != null)
                {
                    for (int i = 0; i < breakIdentifier.Length; i++)
                    {
                        if (data[currentIndex] == breakIdentifier[i])
                        {
                            return -1;
                        }
                    }
                }

                bool isMatch = false;
                for (int i = 0; i < identifier.Length; i += offset)
                {
                    bool isMatchAll = true;
                    for (int f = 0; f < offset; f++)
                    {
                        if (data[currentIndex - offset + f] != identifier[i + f])
                        {
                            isMatchAll = false;
                            break;
                        }
                    }
                    if (isMatchAll)
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch)
                {
                    return currentIndex - offset;
                }

                currentIndex--;
            }
            return -1;
        }

        private int LookForward(int index, char[] identifier, int offset, char[] breakIdentifier)
        {
            int currentIndex = index;
            while (length - currentIndex >= offset)
            {
                if (breakIdentifier != null)
                {
                    for (int i = 0; i < breakIdentifier.Length; i++)
                    {
                        if (data[currentIndex] == breakIdentifier[i])
                        {
                            return -1;
                        }
                    }
                }

                bool isMatch = false;
                for (int i = 0; i < identifier.Length; i += offset)
                {
                    bool isMatchAll = true;
                    for (int f = 0; f < offset; f++)
                    {
                        if (data[currentIndex + f] != identifier[i + f])
                        {
                            isMatchAll = false;
                            break;
                        }
                    }
                    if (isMatchAll)
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch)
                {
                    return currentIndex + offset;
                }

                currentIndex++;
            }
            return -1;
        }

        private string Upper(string source)
        {
            if (!source.IsNullOrWhiteSpace())
            {
                return source[0].ToString().ToUpper() + source.Substring(1);
            }

            return source;
        }

        private string TypeConvert(string source)
        {
            if (!source.IsNullOrWhiteSpace() && typeMapper.ContainsKey(source))
            {
                return typeMapper[source];
            }

            return source;
        }
    }
}
