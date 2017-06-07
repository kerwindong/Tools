using System;
using System.Collections.Generic;
using System.Text;
using StringUtility.Common.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class JavaMapperProperty
    {
        public string TypeName { set; get; }

        public string PropertyName { set; get; }
    }

    public class JavaMapper : IUtility
    {
        private static readonly char[] IDENTIFIER_BREAKER = new char[] { '}', '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_BEGIN = new char[] { '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_END = new char[] { '}' };
        private static readonly char[] SPLIT = new char[] { ';' };
        private static readonly char[] SPACE = new char[] { ' ' };
        private static readonly char[] NEWLINE = new char[] { '\n' };

        private static readonly char[] CLASS_IDENTIFIER = new char[] { 'c', 'l', 'a', 's', 's' };
        private static readonly char[] PROPERTY_IDENTIFIER_SCOPE = new char[] { 'p', 'r', 'i', 'v', 'a', 't', 'e' };

        private static readonly char[] PROTOCONTRACT_IDENTIFIER = new char[] { '[', 'P', 'r', 'o', 't', 'o', 'C', 'o', 'n', 't', 'r', 'a', 'c', 't', ']' };

        private const string TAB = "    ";
        private const string MAPPER = "Java Mapper";
        private const string TARGET_CLASS_NAME = "Target name(class):";
        private const string PROPERTY_MAPPER_FORMAT = "                target.set{0}(source.get{0}());";

        private const string PROPERTY_FORMAT = "        private {0} {1};";

        private const string PROPERTY_SET_FORMAT = "        public void set{1}({0} {2})\n        {{\n            this.{2} = {2};\n        }}";

        private const string PROPERTY_GET_FORMAT = "        public {0} get{1}()\n        {{\n            return {2};\n        }}";

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

        private string mapperFormatterValueClass = string.Empty;

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

        public JavaMapper()
        {
            name = MAPPER;

            MainName = "Generate Java Code";

            AdvanceName = "";

            mapperFormatterValue = ConfigManager.Get().MapperConfig.MapperFormatter.Find(d => d.Type == "Java").Value;

            mapperFormatterValueClass = ConfigManager.Get().MapperConfig.MapperFormatter.Find(d => d.Type == "JavaClass").Value;

            hasOtherInputs = true;

            otherInputsText = TARGET_CLASS_NAME;
        }

        public string Main(string str, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

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

                lookForwardIndex = GetPropertyName(lookForwardIndex);

                lookForwardIndex++;
            }

            var classBuilder = new StringBuilder();

            foreach (var propertyName in propertyNames)
            {
                classBuilder.AppendLine(string.Format(PROPERTY_FORMAT, propertyName.TypeName, propertyName.PropertyName));
                classBuilder.AppendLine();
            }

            foreach (var propertyName in propertyNames)
            {
                classBuilder.AppendLine(string.Format(PROPERTY_GET_FORMAT, propertyName.TypeName, Upper(propertyName.PropertyName), propertyName.PropertyName));
                classBuilder.AppendLine();

                classBuilder.AppendLine(string.Format(PROPERTY_SET_FORMAT, propertyName.TypeName, Upper(propertyName.PropertyName), propertyName.PropertyName));
                classBuilder.AppendLine();
            }

            var classRaw = string.Format(mapperFormatterValueClass, className, classBuilder.ToString());

            builder.AppendLine(classRaw);

            builder.AppendLine();
            builder.AppendLine("----------------------------");
            builder.AppendLine();

            // Mapper 

            var mapperBuilder = new StringBuilder();

            foreach (var propertyName in propertyNames)
            {
                mapperBuilder.AppendLine(string.Format(PROPERTY_MAPPER_FORMAT, Upper(propertyName.PropertyName)));
            }

            var targetClassName = "Target_" + className;

            if (args != null && args.Length > 0)
            {
                targetClassName = args[0];
            }

            var mapper = string.Format(mapperFormatterValue, className, targetClassName, mapperBuilder.ToString());

            builder.AppendLine(mapper);

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
            if (length >= startIndex + CLASS_IDENTIFIER.Length)
            {
                int classNameStart = LookForward(startIndex, CLASS_IDENTIFIER, 5, IDENTIFIER_BREAKER);

                if (classNameStart >= 0)
                {
                    int wrapperStart = LookForward(startIndex, IDENTIFIER_WRAPPER_BEGIN, 1, IDENTIFIER_WRAPPER_END);

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
            if (length >= startIndex + PROPERTY_IDENTIFIER_SCOPE.Length)
            {
                int propertyNameStart = LookForward(startIndex, PROPERTY_IDENTIFIER_SCOPE, 7, IDENTIFIER_BREAKER);

                if (propertyNameStart >= 0)
                {
                    int wrapperStart = LookForward(startIndex, SPLIT, 1, NEWLINE);

                    if (wrapperStart > propertyNameStart)
                    {
                        var propertyNameFull = data.Substring(propertyNameStart, wrapperStart - propertyNameStart - 1).Trim();

                        var propertyNameFulls = propertyNameFull.Split(' ');

                        if (propertyNameFulls.Length >= 1)
                        {
                            propertyNames.Add(new MapperProperty()
                            {
                                PropertyName = propertyNameFulls[1].Trim(),
                                TypeName = TypeConvert(propertyNameFulls[0].Trim())
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
