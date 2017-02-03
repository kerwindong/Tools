using System;
using System.IO;
using System.Text;

namespace StringUtility
{
    public class GoogleProtoUtility : IUtility
    {
        private static readonly char[] IDENTIFIER_BREAKER = new char[] { '}', ';', '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_BEGIN = new char[] { '{' };
        private static readonly char[] IDENTIFIER_WRAPPER_END = new char[] { '}' };
        private static readonly char[] IDENTIFIER_WRAPPER_OVER = new char[] { ';' };

        private static readonly char[] CLASS_IDENTIFIER = new char[] { 'c', 'l', 'a', 's', 's' };
        private static readonly char[] CLASS_IDENTIFIER_SCOPE = new char[] { 'p', 'u', 'b', 'l', 'i', 'c' };

        private static readonly char[] PROPERTY_SURFIX = new char[] { ';' };
        private static readonly char[] PROPERTY_IDENTIFIER_SCOPE = new char[] { 'p', 'u', 'b', 'l', 'i', 'c' };
        private static readonly char[] PROPERTY_IDENTIFIER_BREAKER = new char[] { '(', ')', '{', '}' };
        private static readonly char[] NEWLINE_IDENTIFIER = new char[] { '\r', '\n' };

        private static readonly char[] PROPERTY_SET_GET_IDENTIFIER = new char[] { 's', 'e', 't', 'g', 'e', 't' };
        private static readonly char[] PROPERTY_SET_GET_BREAK = new char[] { ';', '{', '}' };
        private static readonly char[] PROPERTY_SET_GET_WRAPPER_PREFIX_IDENTIFIER = new char[] { '{' };
        private static readonly char[] PROPERTY_SET_GET_WRAPPER_PREFIX_BREAK = new char[] { ';', '}' };

        private static readonly char[] PROTOCONTRACT_IDENTIFIER = new char[] { '[', 'P', 'r', 'o', 't', 'o', 'C', 'o', 'n', 't', 'r', 'a', 'c', 't', ']' };
        private static readonly char[] PROTOMEMBER_IDENTIFIER_PREFIX = new char[] { '[', 'P', 'r', 'o', 't', 'o', 'M', 'e', 'm', 'b', 'e', 'r', '(' };
        private static readonly char[] PROTOMEMBER_IDENTIFIER_SURFIX = new char[] { ')', ']' };

        private static readonly char[] PROTO_USING_IDENTIFIER = new char[] { 'u', 's', 'i', 'n', 'g', ' ', 'P', 'r', 'o', 't', 'o', 'B', 'u', 'f', ';' };
        private static readonly char[] PROTO_USING_PREFIX_IDENTIFIER = new char[] { 'n', 'a', 'm', 'e', 's', 'p', 'a', 'c', 'e' };

        private const string TAB = "    ";
        private const string TAB_DOUBLE = "        ";

        private string data = string.Empty;
        private string dataOut = string.Empty;
        private StringBuilder builder = default(StringBuilder);
        private int length = 0;
        private int buildIndex = 0;
        private int lookForwardIndex = 0;
        private int memberIndex = 1;

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

        public GoogleProtoUtility()
        {
            Name = "Google Proto";

            MainName = "Add Attribute";

            AdvanceName = "";
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
                int addUsingIndex = AddUsing(lookForwardIndex);
                if (addUsingIndex >= 0)
                {
                    Append(buildIndex, addUsingIndex - buildIndex);
                    AppendProtoUsing();
                    buildIndex = addUsingIndex;
                    lookForwardIndex++;
                    continue;
                }

                // Add class identifier
                int addClassAttributeIndex = AddAttributeOfClass(lookForwardIndex);
                if (addClassAttributeIndex >= 0)
                {
                    Append(buildIndex, addClassAttributeIndex - buildIndex);
                    AppendProtoContract();
                    buildIndex = addClassAttributeIndex;
                    lookForwardIndex++;
                    continue;
                }

                int addPropertyAttributeIndex = AddAttributeOfProperty(lookForwardIndex);
                if (addPropertyAttributeIndex >= 0)
                {
                    Append(buildIndex, addPropertyAttributeIndex - buildIndex);
                    AppendProtoMember();
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
            return "not implemented";
        }

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        private int AddUsing(int startIndex)
        {
            while (length >= startIndex + PROTO_USING_PREFIX_IDENTIFIER.Length)
            {
                for (int i = 0; i < PROTO_USING_PREFIX_IDENTIFIER.Length; i++)
                {
                    if (data[startIndex + i] != PROTO_USING_PREFIX_IDENTIFIER[i])
                    {
                        return -1;
                    }
                }

                int usingStartIndex = LookBackward(startIndex, PROTO_USING_IDENTIFIER, 15, IDENTIFIER_BREAKER);
                if (usingStartIndex < 0)
                {
                    int newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_BREAKER);
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

        private int AddAttributeOfClass(int startIndex)
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

                int scopeStartIndex = LookBackward(startIndex, CLASS_IDENTIFIER_SCOPE, 6, IDENTIFIER_BREAKER);
                if (scopeStartIndex >= 0)
                {
                    int attributeStartIndex = LookBackward(scopeStartIndex, PROTOCONTRACT_IDENTIFIER, 15, IDENTIFIER_BREAKER);
                    if (attributeStartIndex < 0)
                    {
                        int newLineStartIndex = LookBackward(scopeStartIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_BREAKER);
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
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        private int AddAttributeOfProperty(int startIndex)
        {
            int forwardIndex = startIndex + PROPERTY_IDENTIFIER_SCOPE.Length;

            while (length >= forwardIndex)
            {
                for (int i = 0; i < PROPERTY_IDENTIFIER_SCOPE.Length; i++)
                {
                    if (data[startIndex + i] != PROPERTY_IDENTIFIER_SCOPE[i])
                    {
                        return -1;
                    }
                }

                int classStartIndex = LookForward(forwardIndex, CLASS_IDENTIFIER, 5, IDENTIFIER_BREAKER);
                if (classStartIndex >= 0)
                {
                    return -1;
                }

                // normal public member
                int surfixStartIndex = LookForward(forwardIndex, PROPERTY_SURFIX, 1, PROPERTY_IDENTIFIER_BREAKER);
                if (surfixStartIndex >= 0)
                {
                    int attributeStartIndex = LookBackward(startIndex, PROTOMEMBER_IDENTIFIER_PREFIX, 13, IDENTIFIER_BREAKER);
                    if (attributeStartIndex < 0)
                    {
                        int newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_BREAKER);
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
                                int attributeStartIndex = LookBackward(startIndex, PROTOMEMBER_IDENTIFIER_PREFIX, 13, IDENTIFIER_BREAKER);
                                if (attributeStartIndex < 0)
                                {
                                    int newLineStartIndex = LookBackward(startIndex, NEWLINE_IDENTIFIER, 2, IDENTIFIER_BREAKER);
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

        private void AppendProtoUsing()
        {
            for (int i = 0; i < PROTO_USING_IDENTIFIER.Length; i++)
            {
                builder.Append(PROTO_USING_IDENTIFIER[i]);
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

        private void AppendProtoMember()
        {
            builder.Append(TAB_DOUBLE);
            for (int i = 0; i < PROTOMEMBER_IDENTIFIER_PREFIX.Length; i++)
            {
                builder.Append(PROTOMEMBER_IDENTIFIER_PREFIX[i]);
            }
            builder.Append(memberIndex);
            for (int i = 0; i < PROTOMEMBER_IDENTIFIER_SURFIX.Length; i++)
            {
                builder.Append(PROTOMEMBER_IDENTIFIER_SURFIX[i]);
            }
            memberIndex++;
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
