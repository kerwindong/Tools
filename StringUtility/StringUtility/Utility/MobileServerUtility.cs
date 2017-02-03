
using System;
using System.Collections.Generic;
using System.Text;

using StringUtility.Common.Text;

namespace StringUtility.Utility
{
    public class MobileServerUtility : IUtility
    {
        #region Const

        private const string TAB = "    ";
        private const string TAB_DOUBLE = "        ";

        #endregion

        #region Static

        private static readonly char[] IDENTIFIER_ESCAPE = new char[] { '}', ';', '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_BEGIN = new char[] { '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_END = new char[] { '}' };
        private static readonly char[] IDENTIFIER_WRAPPER_OVER = new char[] { ';' };

        private static readonly char[] CLASS_IDENTIFIER = new char[] { 'c', 'l', 'a', 's', 's' };
        private static readonly char[] CLASS_IDENTIFIER_SCOPE = new char[] { 'p', 'u', 'b', 'l', 'i', 'c' };

        private static readonly char[] ENUM_IDENTIFIER = new char[] { 'e', 'n', 'u', 'm' };

        private static readonly char[] PROPERTY_SURFIX = new char[] { ';' };
        private static readonly char[] PROPERTY_IDENTIFIER_SCOPE = new char[] { 'p', 'u', 'b', 'l', 'i', 'c' };
        private static readonly char[] PROPERTY_IDENTIFIER_ESCAPE = new char[] { '(', ')', '{', '}' };
        private static readonly char[] PROPERTY_IDENTIFIER_SPLIT = new char[] { ' ' };
        private static readonly char[] NEWLINE_IDENTIFIER = new char[] { '\r', '\n' };

        private static readonly char[] PROPERTY_SET_GET_IDENTIFIER = new char[] { 's', 'e', 't', 'g', 'e', 't' };
        private static readonly char[] PROPERTY_SET_GET_BREAK = new char[] { ';', '{', '}' };
        private static readonly char[] PROPERTY_SET_GET_WRAPPER_PREFIX_IDENTIFIER = new char[] { '{' };
        private static readonly char[] PROPERTY_SET_GET_WRAPPER_PREFIX_BREAK = new char[] { ';', '}' };

        private static readonly char[] DATAGRAMFIELD_IDENTIFIER_PREFIX = "[DatagramField(".ToCharArray();
        private static readonly char[] DATAGRAMFIELD_IDENTIFIER_SURFIX = new char[] { ')', ']' };
        private static readonly string DATAGRAMFIELD_FORMAT = "Type = FieldType.{0}, Version = 1, Require = false, Description = \"\"";

        private static readonly char[] MOBILE_UTILITY_USING_IDENTIFIER = "using Ctrip.Mobile.Utility.Common;".ToCharArray();
        private static readonly char[] USING_PREFIX_IDENTIFIER = new char[] { 'n', 'a', 'm', 'e', 's', 'p', 'a', 'c', 'e' };

        private static readonly string NULLABLE_ENUM_FORMAT = "System.Nullable<{0}>";

        private static readonly Dictionary<string, MobileServerFieldType> CommonTypeMappings = new Dictionary<string, MobileServerFieldType>()
        {
            { "string" , MobileServerFieldType.Dynamic10 }, 

            { "String" , MobileServerFieldType.Dynamic10 }, 

            { "bool", MobileServerFieldType.Boolean }, 

            { "Boolean", MobileServerFieldType.Boolean }, 

            { "DateTime", MobileServerFieldType.DateTime }, 

            { "short", MobileServerFieldType.Int4 }, 

            { "Int16", MobileServerFieldType.Int4 }, 

            { "int", MobileServerFieldType.Int10 }, 

            { "Int32", MobileServerFieldType.Int10 }, 

            { "long", MobileServerFieldType.Int20 }, 

            { "Int64", MobileServerFieldType.Int20 }, 

            { "decimal", MobileServerFieldType.Decimal6 }, 
        };

        private static readonly Dictionary<string, MobileServerFieldType> StartWithTypeMappings = new Dictionary<string, MobileServerFieldType>()
        {
            { "List<", MobileServerFieldType.List }, 
        };

        private static readonly Dictionary<string, MobileServerFieldType> EndWithTypeMappings = new Dictionary<string, MobileServerFieldType>()
        {
            { "[]", MobileServerFieldType.List }, 
        };

        #endregion 

        #region Fields

        private string data = string.Empty;

        private string dataOut = string.Empty;

        private StringBuilder builder = default(StringBuilder);

        private int length = 0;

        private int buildIndex = 0;

        private int lookForwardIndex = 0;

        private Dictionary<string, string> enumNames = new Dictionary<string, string>();  

        #endregion

        #region Contractor

        public MobileServerUtility()
        {
            Name = "Mobile Server";

            MainName = "Add DatagramField";

            AdvanceName = "";

            HasOtherInputs = false;

            OtherInputsText = string.Empty;
        }

        #endregion

        #region Properties

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public bool HasOtherInputs { set; get; }

        public string OtherInputsText { set; get; }

        #endregion

        #region Public methods

        public string Main(string str, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            data = string.Empty;

            data = str;

            length = data.Length;

            lookForwardIndex = 0;

            var enumNameBuilder = new StringBuilder();

            while (true)
            {
                if (lookForwardIndex >= length)
                {
                    break;
                }

                // Identify Enum name
                var enumBeginIndex = LookForward(lookForwardIndex, ENUM_IDENTIFIER, ENUM_IDENTIFIER.Length, IDENTIFIER_ESCAPE);

                if (enumBeginIndex >= 0)
                {
                    var enumNameBegin = enumBeginIndex;

                    var enumNameEnd = LookForward(enumBeginIndex, NEWLINE_IDENTIFIER, NEWLINE_IDENTIFIER.Length, null);

                    if (enumNameBegin > 0 && enumNameEnd > enumNameBegin)
                    {
                        var enumName = string.Empty;

                        for (var i = enumNameBegin; i < enumNameEnd; i++)
                        {
                            enumNameBuilder.Append(data[i]);
                        }

                        enumName = enumNameBuilder.ToString().Trim();

                        enumNameBuilder.Clear();

                        if (!enumName.IsNullOrWhiteSpace() && !enumNames.ContainsKey(enumName))
                        {
                            enumNames.Add(enumName, enumName);

                            enumNames.Add(string.Format(NULLABLE_ENUM_FORMAT, enumName), enumName);
                        }
                    }
                }

                lookForwardIndex++;
            }

            lookForwardIndex = 0;

            buildIndex = 0;

            dataOut = string.Empty;

            var propertyTypeNameBuilder = new StringBuilder();

            while (true)
            {
                if (lookForwardIndex >= length)
                {
                    break;
                }

                if (builder == null)
                {
                    builder = new StringBuilder();
                }

                // Add using
                var addUsingIndex = AddUsing(lookForwardIndex);

                if (addUsingIndex >= 0)
                {
                    Append(buildIndex, addUsingIndex - buildIndex);

                    builder.Append(MOBILE_UTILITY_USING_IDENTIFIER);
                    builder.Append(Environment.NewLine);

                    buildIndex = addUsingIndex;
                    lookForwardIndex++;
                    continue;
                }

                var addPropertyAttributeIndex = AddAttributeOfProperty(lookForwardIndex);

                if (addPropertyAttributeIndex >= 0)
                {
                    var propertyTypeNameBegin = LookForward(lookForwardIndex, PROPERTY_IDENTIFIER_SCOPE, PROPERTY_IDENTIFIER_SCOPE.Length, IDENTIFIER_ESCAPE);

                    var propertyTypeNameEnd = 0;

                    if (propertyTypeNameBegin > 0)
                    {
                        propertyTypeNameBegin = LookForward(propertyTypeNameBegin, PROPERTY_IDENTIFIER_SPLIT, PROPERTY_IDENTIFIER_SPLIT.Length, null);
                    }

                    if (propertyTypeNameBegin > 0)
                    {
                        propertyTypeNameEnd = LookForward(propertyTypeNameBegin, PROPERTY_IDENTIFIER_SPLIT, PROPERTY_IDENTIFIER_SPLIT.Length, null);
                    }

                    var fieldTypeName = string.Empty;

                    if (propertyTypeNameBegin > 0 && propertyTypeNameEnd > propertyTypeNameBegin)
                    {
                        var propertyTypeName = string.Empty;

                        for (var i = propertyTypeNameBegin; i < propertyTypeNameEnd; i++)
                        {
                            propertyTypeNameBuilder.Append(data[i]);
                        }

                        propertyTypeName = propertyTypeNameBuilder.ToString().Trim();

                        propertyTypeNameBuilder.Clear();

                        if (!propertyTypeName.IsNullOrWhiteSpace())
                        {
                            fieldTypeName = GetMobileServerFieldType(propertyTypeName).ToString();
                        }
                    }

                    Append(buildIndex, addPropertyAttributeIndex - buildIndex);

                    builder.Append(TAB_DOUBLE);

                    builder.Append(DATAGRAMFIELD_IDENTIFIER_PREFIX);

                    builder.AppendFormat(DATAGRAMFIELD_FORMAT, fieldTypeName);

                    builder.Append(DATAGRAMFIELD_IDENTIFIER_SURFIX);

                    buildIndex = addPropertyAttributeIndex;

                    lookForwardIndex++;

                    continue;
                }

                lookForwardIndex++;
            }

            Append(buildIndex, length - buildIndex);

            if (builder != null)
            {
                dataOut = builder.ToString();

                builder.Clear();
            }

            return dataOut;
        }

        public string Advance(string str, params string[] args)
        {
            return string.Empty;
        }

        #endregion

        #region Private methods

        private int AddUsing(int startIndex)
        {
            while (length >= startIndex + USING_PREFIX_IDENTIFIER.Length)
            {
                for (int i = 0; i < USING_PREFIX_IDENTIFIER.Length; i++)
                {
                    if (data[startIndex + i] != USING_PREFIX_IDENTIFIER[i])
                    {
                        return -1;
                    }
                }

                int usingStartIndex = LookBackward(startIndex, MOBILE_UTILITY_USING_IDENTIFIER, MOBILE_UTILITY_USING_IDENTIFIER.Length, IDENTIFIER_ESCAPE);
                if (usingStartIndex < 0)
                {
                    int newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_ESCAPE);
                    if (newLineStartIndex >= 0)
                    {
                        return newLineStartIndex;
                    }
                    return startIndex;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        private int LookupClass(int startIndex)
        {
            while (length >= startIndex + CLASS_IDENTIFIER.Length)
            {
                for (int i = 0; i < CLASS_IDENTIFIER.Length; i++)
                {
                    if (data[startIndex + i] != CLASS_IDENTIFIER[i])
                    {
                        return -1;
                    }
                }

                var scopeStartIndex = LookBackward(startIndex, CLASS_IDENTIFIER_SCOPE, 6, IDENTIFIER_ESCAPE);

                if (scopeStartIndex >= 0)
                {
                    var newLineStartIndex = LookBackward(scopeStartIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_ESCAPE);

                    if (newLineStartIndex >= 0)
                    {
                        return newLineStartIndex;
                    }

                    return scopeStartIndex;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        private int AddAttributeOfProperty(int startIndex)
        {
            var forwardIndex = startIndex + PROPERTY_IDENTIFIER_SCOPE.Length;

            while (length >= forwardIndex)
            {
                for (int i = 0; i < PROPERTY_IDENTIFIER_SCOPE.Length; i++)
                {
                    if (data[startIndex + i] != PROPERTY_IDENTIFIER_SCOPE[i])
                    {
                        return -1;
                    }
                }

                var classStartIndex = LookForward(forwardIndex, CLASS_IDENTIFIER, 5, IDENTIFIER_ESCAPE);

                if (classStartIndex >= 0)
                {
                    return -1;
                }

                // Common Public Property 
                var surfixStartIndex = LookForward(forwardIndex, PROPERTY_SURFIX, 1, PROPERTY_IDENTIFIER_ESCAPE);

                if (surfixStartIndex >= 0)
                {
                    var attributeStartIndex = LookBackward(startIndex, DATAGRAMFIELD_IDENTIFIER_PREFIX, DATAGRAMFIELD_IDENTIFIER_PREFIX.Length, IDENTIFIER_ESCAPE);

                    if (attributeStartIndex < 0)
                    {
                        var newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_ESCAPE);

                        if (newLineStartIndex >= 0)
                        {
                            return newLineStartIndex;
                        }
                        return startIndex;
                    }
                    else
                    {
                        return -1;
                    }
                }

                // SET GET Member
                int setgetWrapperPrefixStartIndex = LookForward(forwardIndex, PROPERTY_SET_GET_WRAPPER_PREFIX_IDENTIFIER, 1, PROPERTY_SET_GET_WRAPPER_PREFIX_BREAK);
                if (setgetWrapperPrefixStartIndex >= 0)
                {
                    int setgetStartIndex = LookForward(setgetWrapperPrefixStartIndex, PROPERTY_SET_GET_IDENTIFIER, 3, PROPERTY_SET_GET_BREAK);
                    if (setgetStartIndex >= 0)
                    {
                        int wrapperIndex = -1;

                        // get { }
                        int convertationBeginIndex = LookForward(setgetStartIndex, IDENTIFIER_WRAPPER_BEGIN, 1, IDENTIFIER_WRAPPER_END);
                        if (convertationBeginIndex >= 0)
                        {
                            int wrapperEndIndex = LookForward(convertationBeginIndex, IDENTIFIER_WRAPPER_END, 1, null);
                            if (wrapperEndIndex >= 0)
                            {
                                wrapperIndex = wrapperEndIndex;
                            }
                        }

                        // get;
                        if (wrapperIndex < 0)
                        {
                            int wrapperOverIndex = LookForward(setgetStartIndex, IDENTIFIER_WRAPPER_OVER, 1, null);
                            if (wrapperOverIndex >= 0)
                            {
                                wrapperIndex = wrapperOverIndex;
                            }
                        }

                        if (wrapperIndex > 0)
                        {
                            int setgetAnotherIndex = LookForward(wrapperIndex, PROPERTY_SET_GET_IDENTIFIER, 3, PROPERTY_SET_GET_BREAK);
                            if (setgetAnotherIndex >= 0)
                            {
                                int attributeStartIndex = LookBackward(startIndex, DATAGRAMFIELD_IDENTIFIER_PREFIX, DATAGRAMFIELD_IDENTIFIER_PREFIX.Length, IDENTIFIER_ESCAPE);
                                if (attributeStartIndex < 0)
                                {
                                    int newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_ESCAPE);
                                    if (newLineStartIndex >= 0)
                                    {
                                        return newLineStartIndex;
                                    }
                                    return startIndex;
                                }
                                else
                                {
                                    return -1;
                                }
                            }
                            else
                            {
                                return -1;
                            }
                        }

                        return -1;
                    }
                }

                return -1;
            }

            return -1;
        }

        private void Append(int startIndex, int length)
        {
            for (int i = 0; i < length; i++)
            {
                builder.Append(data[startIndex + i]);
            }
            builder.Append(Environment.NewLine);
        }

        private int LookBackward(int index, char[] identifier, int offset, char[] escape)
        {
            int currentIndex = index;
            while (currentIndex >= offset)
            {
                if (escape != null)
                {
                    for (int i = 0; i < escape.Length; i++)
                    {
                        if (data[currentIndex] == escape[i])
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

        private MobileServerFieldType GetMobileServerFieldType(string typeName)
        {
            var target = MobileServerFieldType.NullableClass;

            if (typeName.IsNullOrWhiteSpace())
            {
                return target;
            }

            if (enumNames.ContainsKey(typeName))
            {
                return MobileServerFieldType.Enum;
            }

            if (CommonTypeMappings.ContainsKey(typeName))
            {
                return CommonTypeMappings[typeName];
            }

            foreach (var key in StartWithTypeMappings.Keys)
            {
                if (typeName.StartsWith(key))
                {
                    return StartWithTypeMappings[key];
                }
            }

            foreach (var key in EndWithTypeMappings.Keys)
            {
                if (typeName.EndsWith(key))
                {
                    return EndWithTypeMappings[key];
                }
            }

            return target;
        }

        #endregion
    }

    public enum MobileServerFieldType
    {
        Invalid,

        /// <summary>
        /// string 
        /// </summary>
        Dynamic10, 

        /// <summary>
        /// List&lt;Class&gt;
        /// </summary>
        List,

        /// <summary>
        /// Enum
        /// </summary>
        Enum,

        /// <summary>
        /// Class 
        /// </summary>
        NullableClass,

        /// <summary>
        /// bool 
        /// </summary>
        Boolean, 

        /// <summary>
        /// DateTime 
        /// </summary>
        DateTime,

        /// <summary>
        /// short
        /// </summary>
        Int4,

        /// <summary>
        /// int 
        /// </summary>
        Int10,

        /// <summary>
        /// long 
        /// </summary>
        Int20,

        /// <summary>
        /// decimal 
        /// </summary>
        Decimal6, 
        
    }
}
