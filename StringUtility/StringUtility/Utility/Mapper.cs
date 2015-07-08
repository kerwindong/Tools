using System;
using System.Collections.Generic;
using System.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class MapperProperty
    {
        public string TypeName { set; get; }

        public string PropertyName { set; get; }
    }

    public class Mapper : IUtility
    {
        private static readonly char[] IDENTIFIER_BREAKER = new char[] { '}', ';', '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_BEGIN = new char[] { '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_END = new char[] { '}' };

        private static readonly char[] CLASS_IDENTIFIER = new char[] { 'c', 'l', 'a', 's', 's' };
        private static readonly char[] PROPERTY_IDENTIFIER_SCOPE = new char[] { 'p', 'u', 'b', 'l', 'i', 'c' };

        private static readonly char[] PROTOCONTRACT_IDENTIFIER = new char[] { '[', 'P', 'r', 'o', 't', 'o', 'C', 'o', 'n', 't', 'r', 'a', 'c', 't', ']' };

        private const string TAB = "    ";
        private const string MAPPER = "Mapper";
        private const string TARGET_CLASS_NAME = "Target name(class):";
        private const string PROPERTY_MAPPER_FORMAT = "                target.{0} = source.{0};";

        private const string PROPERTY_SETGET_FORMAT = "public {0} {1} {{ set; get; }}";

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

        public Mapper()
        {
            name = MAPPER;

            MainName = "Generate Code";

            AdvanceName = "";

            mapperFormatterValue = ConfigManager.Get().MapperConfig.MapperFormatter.Value;

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

            var mapperBuilder = new StringBuilder();

            foreach (var propertyName in propertyNames)
            {
                mapperBuilder.AppendLine(string.Format(PROPERTY_MAPPER_FORMAT, propertyName.PropertyName));
            }

            var targetClassName = "Target_" + className;

            if (args != null && args.Length > 0)
            {
                targetClassName = args[0];
            }

            var mapper = string.Format(mapperFormatterValue, className, targetClassName, mapperBuilder.ToString());

            builder.AppendLine(mapper);

            builder.AppendLine();
            builder.AppendLine("----------------------------");
            builder.AppendLine();

            foreach (var propertyName in propertyNames)
            {
                builder.AppendLine(string.Format(PROPERTY_SETGET_FORMAT, propertyName.TypeName, propertyName.PropertyName));
                builder.AppendLine();
            }

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
                int propertyNameStart = LookForward(startIndex, PROPERTY_IDENTIFIER_SCOPE, 6, IDENTIFIER_BREAKER);

                if (propertyNameStart >= 0)
                {
                    int wrapperStart = LookForward(startIndex, IDENTIFIER_WRAPPER_BEGIN, 1, IDENTIFIER_WRAPPER_END);

                    if (wrapperStart > propertyNameStart)
                    {
                        var propertyNameFull = data.Substring(propertyNameStart, wrapperStart - propertyNameStart - 1).Trim();

                        var propertyNameFulls = propertyNameFull.Split(' ');

                        if (propertyNameFulls.Length >= 1)
                        {
                            propertyNames.Add(new MapperProperty()
                            {
                                PropertyName = propertyNameFulls[1].Trim(),
                                TypeName = propertyNameFulls[0].Trim()
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
    }
}
