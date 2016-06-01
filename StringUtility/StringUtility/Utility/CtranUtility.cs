
using System.Collections.Generic;
using System.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class CtranCoder
    {
        public string Key { set; get; }

        public string Value { set; get; }

        public int ConvertionBegin { set; get; }

        public int ConvertionEnd { set; get; }

        public int KeyBegin { set; get; }

        public int KeyEnd { set; get; }

        public int ValueBegin { set; get; }

        public int ValueEnd { set; get; }

        public ConvertionType ConvertionType { set; get; }
    }

    public enum ConvertionType
    {
        Invalid,

        /// <summary>
        /// <a key="KEY">VALUE</a>
        /// </summary>
        Key,

        /// <summary>
        /// <a><tag key="KEY">VALUE</tag><br/></a>
        /// </summary>
        Tag,

        /// <summary>
        /// <input key="KEY" value="VALUE" type="Submit" />
        /// </summary>
        InputValue,

        /// <summary>
        /// <input key="KEY" value="VALUE" type="Text" />
        /// </summary>
        InputPlaceHolder
    }

    public class CtranUtility : IUtility
    {

        private static readonly char[] ELEMENT_BEGIN = new char[] { '<' };
        private static readonly char[] ELEMENT_END = new char[] { '>' };

        private static readonly char[] CKEY_INPUT_IDENTIFIER = new char[] { 'i', 'n', 'p', 'u', 't' };

        private static readonly char[] CKEY_INPUT_VALUE_1_IDENTIFIER = new char[] { 'v', 'a', 'l', 'u', 'e' };
        private static readonly char[] CKEY_INPUT_VALUE_2_IDENTIFIER = new char[] { '=' };
        private static readonly char[] CKEY_INPUT_VALUE_3_IDENTIFIER = new char[] { '"' };

        private static readonly char[] CKEY_INPUT_PLACE_HOLDER_1_IDENTIFIER = new char[] { 'p', 'l', 'a', 'c', 'e', 'h', 'o', 'l', 'd', 'e', 'r' };
        private static readonly char[] CKEY_INPUT_PLACE_HOLDER_2_IDENTIFIER = new char[] { '=' };
        private static readonly char[] CKEY_INPUT_PLACE_HOLDER_3_IDENTIFIER = new char[] { '"' };

        private static readonly char[] CKEY_TAG_1_IDENTIFIER = new char[] { '<', 't', 'a', 'g' };

        private static readonly char[] CKEY_PREFIX_1_IDENTIFIER = new char[] { ' ', 'c', 'k', 'e', 'y' };
        private static readonly char[] CKEY_PREFIX_2_IDENTIFIER = new char[] { '=' };
        private static readonly char[] CKEY_PREFIX_3_IDENTIFIER = new char[] { '"' };
        private static readonly char[] CKEY_SURFIX_1_IDENTIFIER = new char[] { '"' };

        private static readonly char[] CVALUE_PREFIX_1_IDENTIFIER = new char[] { '>' };
        private static readonly char[] CVALUE_SURFIX_1_IDENTIFIER = new char[] { '<' };

        private static readonly char[] CKEY_TAG_2_IDENTIFIER = new char[] { '/', 't', 'a', 'g', '>' };

        private static readonly char[] CKEY_TAG_END_1_IDENTIFIER = new char[] { '<', '/', 't', 'a', 'g', '>' };

        private static readonly char[] CKEY_TAG_END_2_IDENTIFIER = new char[] { '\n' };

        private const string CODE_FORMAT = "@resource.{0}";

        private const string KEY_VALUE_FORMAT = "\"{0}\":\"{1}\", ";

        private const string CTRAN = "Ctran Coder";

        private string name;

        private string data = string.Empty;

        private string dataOut = string.Empty;

        private StringBuilder builder = default(StringBuilder);

        private int length = 0;

        private int lookForwardIndex = 0;

        private List<CtranCoder> ctranCoders = default(List<CtranCoder>);

        private bool hasOtherInputs = false;

        private string otherInputsText = "";

        private string ctranFormat = string.Empty;

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

        public CtranUtility()
        {
            name = CTRAN;

            MainName = "Generate Code";

            AdvanceName = "";

            hasOtherInputs = false;

            otherInputsText = string.Empty;

            ctranFormat = ConfigManager.Get().CtranConfig.CtranFormatter.Value;
        }

        public string Main(string str, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            data = string.Empty;

            dataOut = string.Empty;

            data = str;

            length = data.Length;

            lookForwardIndex = 0;

            ctranCoders = new List<CtranCoder>();

            builder = new StringBuilder();

            while (true)
            {
                if (lookForwardIndex >= length)
                {
                    break;
                }

                lookForwardIndex = GetCKey(lookForwardIndex);

                lookForwardIndex++;
            }

            var convertionBuilder = new StringBuilder();

            var jsonBuilder = new StringBuilder();

            var formatBuilder = new StringBuilder();

            var start = 0;

            if (ctranCoders.Count > 0)
            {
                var ctranCoder = ctranCoders[0];

                if (ctranCoders.Count > 1)
                {
                    if (ctranCoders[1].ValueBegin < ctranCoders[1].KeyBegin)
                    {
                        start = ctranCoders[1].ValueBegin;
                    }
                    else
                    {
                        start = ctranCoders[1].ConvertionBegin;
                    }
                }
                else
                {
                    start = data.Length;
                }

                if (ctranCoder != null)
                {
                    if (ctranCoder.ValueBegin < ctranCoder.KeyBegin)
                    {
                        convertionBuilder.Append(data.Substring(0, ctranCoder.ValueBegin));

                        convertionBuilder.Append(string.Format(CODE_FORMAT, ctranCoder.Key.Replace('.','_')));

                        convertionBuilder.Append(data.Substring(ctranCoder.ValueEnd, ctranCoder.ConvertionBegin - ctranCoder.ValueEnd));

                        convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, start - ctranCoder.ConvertionEnd));
                    }
                    else
                    {
                        convertionBuilder.Append(data.Substring(0, ctranCoder.ConvertionBegin));

                        if (ctranCoder.ConvertionType == ConvertionType.Tag)
                        {
                            // 
                        }
                        else
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, ctranCoder.ValueBegin - ctranCoder.ConvertionEnd));
                        }

                        convertionBuilder.Append(string.Format(CODE_FORMAT, ctranCoder.Key.Replace('.', '_')));

                        if (ctranCoder.ConvertionType == ConvertionType.Tag)
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, start - ctranCoder.ConvertionEnd));
                        }
                        else
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ValueEnd, start - ctranCoder.ValueEnd));
                        }
                    }

                    jsonBuilder.AppendLine(string.Format(KEY_VALUE_FORMAT, ctranCoder.Key, ctranCoder.Value));

                    formatBuilder.AppendLine(string.Format(ctranFormat, ctranCoder.Key, ctranCoder.Value, ctranCoder.Key.Replace('.','_')));
                }
            }

            for (var i = 1; i < ctranCoders.Count; i++)
            {
                var ctranCoder = ctranCoders[i];

                if (i + 1 < ctranCoders.Count)
                {
                    if (ctranCoders[i + 1].ValueBegin < ctranCoders[i + 1].KeyBegin)
                    {
                        start = ctranCoders[i + 1].ValueBegin;
                    }
                    else
                    {
                        start = ctranCoders[i + 1].ConvertionBegin;
                    }
                }
                else
                {
                    start = data.Length;
                }

                if (ctranCoder != null)
                {
                    if (ctranCoder.ValueBegin < ctranCoder.KeyBegin)
                    {
                        convertionBuilder.Append(string.Format(CODE_FORMAT, ctranCoder.Key.Replace('.', '_')));

                        convertionBuilder.Append(data.Substring(ctranCoder.ValueEnd, ctranCoder.ConvertionBegin - ctranCoder.ValueEnd));

                        convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, start - ctranCoder.ConvertionEnd));
                    }
                    else
                    {
                        if (ctranCoder.ConvertionType == ConvertionType.Tag)
                        {
                            // convertionBuilder.Append(data.Substring(ctranCoder.KeyEnd + 1, ctranCoder.ValueBegin - ctranCoder.KeyEnd - 1));
                        }
                        else
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, ctranCoder.ValueBegin - ctranCoder.ConvertionEnd));
                        }

                        convertionBuilder.Append(string.Format(CODE_FORMAT, ctranCoder.Key.Replace('.', '_')));

                        if (ctranCoder.ConvertionType == ConvertionType.Tag)
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ConvertionEnd, start - ctranCoder.ConvertionEnd));
                        }
                        else
                        {
                            convertionBuilder.Append(data.Substring(ctranCoder.ValueEnd, start - ctranCoder.ValueEnd));
                        }
                    }

                    jsonBuilder.AppendLine(string.Format(KEY_VALUE_FORMAT, ctranCoder.Key, ctranCoder.Value));

                    formatBuilder.AppendLine(string.Format(ctranFormat, ctranCoder.Key, ctranCoder.Value, ctranCoder.Key.Replace('.', '_')));
                }
            }

            var convertionOut = convertionBuilder.ToString();

            var jsonOut = jsonBuilder.ToString();

            var formatOut = formatBuilder.ToString();

            if (!string.IsNullOrWhiteSpace(convertionOut) &&
                !string.IsNullOrWhiteSpace(jsonOut) &&
                !string.IsNullOrWhiteSpace(formatOut))
            {
                builder.AppendLine(convertionOut);
                builder.AppendLine("--------------------------------------------------------");
                builder.AppendLine(jsonOut);
                builder.AppendLine("--------------------------------------------------------");
                builder.AppendLine(formatOut);
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

        private int GetCKey(int startIndex)
        {
            if (length >= startIndex + CKEY_PREFIX_1_IDENTIFIER.Length)
            {
                var ckeyPrefix1Begin = LookForward(startIndex, CKEY_PREFIX_1_IDENTIFIER, CKEY_PREFIX_1_IDENTIFIER.Length, ELEMENT_END);

                var convertionBegin = 0;

                var convertionEnd = 0;

                var convertionType = ConvertionType.Key;

                if (ckeyPrefix1Begin > 0)
                {
                    var inputPrefixBegin = LookBackward(ckeyPrefix1Begin, CKEY_INPUT_IDENTIFIER, CKEY_INPUT_IDENTIFIER.Length, ELEMENT_BEGIN);

                    if (inputPrefixBegin > 0)
                    {
                        // <input key="KEY" value="">
                        var ckeyPrefix2Begin = LookForward(ckeyPrefix1Begin, CKEY_PREFIX_2_IDENTIFIER, CKEY_PREFIX_2_IDENTIFIER.Length, ELEMENT_END);

                        if (ckeyPrefix2Begin > 0)
                        {
                            convertionBegin = ckeyPrefix1Begin - CKEY_PREFIX_1_IDENTIFIER.Length + 1;

                            var ckeyBegin = LookForward(ckeyPrefix2Begin, CKEY_PREFIX_3_IDENTIFIER, CKEY_PREFIX_3_IDENTIFIER.Length, ELEMENT_END);

                            if (ckeyBegin > 0)
                            {
                                var ckeyEnd = LookForward(ckeyBegin, CKEY_SURFIX_1_IDENTIFIER, CKEY_SURFIX_1_IDENTIFIER.Length, ELEMENT_END) - 1;

                                if (ckeyEnd > 0)
                                {
                                    convertionEnd = ckeyEnd + 1;
                                }

                                var inputPlaceHolder1Begin = LookForward(inputPrefixBegin, CKEY_INPUT_PLACE_HOLDER_1_IDENTIFIER, CKEY_INPUT_PLACE_HOLDER_1_IDENTIFIER.Length, ELEMENT_END);

                                if (inputPlaceHolder1Begin > 0)
                                {
                                    var inputPlaceHolder2Begin = LookForward(inputPlaceHolder1Begin, CKEY_INPUT_PLACE_HOLDER_2_IDENTIFIER, CKEY_INPUT_PLACE_HOLDER_2_IDENTIFIER.Length, ELEMENT_END);

                                    if (inputPlaceHolder2Begin > 0)
                                    {
                                        var cvalueBegin = LookForward(inputPlaceHolder2Begin, CKEY_INPUT_PLACE_HOLDER_3_IDENTIFIER, CKEY_INPUT_PLACE_HOLDER_3_IDENTIFIER.Length, ELEMENT_END);

                                        if (cvalueBegin > 0)
                                        {
                                            var cvalueEnd = LookForward(cvalueBegin, CKEY_INPUT_PLACE_HOLDER_3_IDENTIFIER, CKEY_INPUT_PLACE_HOLDER_3_IDENTIFIER.Length, ELEMENT_END) - 1;

                                            convertionType = ConvertionType.InputPlaceHolder;

                                            ctranCoders.Add(new CtranCoder()
                                            {
                                                Key = data.Substring(ckeyBegin, ckeyEnd - ckeyBegin),
                                                KeyBegin = ckeyBegin,
                                                KeyEnd = ckeyEnd,
                                                ConvertionBegin = convertionBegin,
                                                ConvertionEnd = convertionEnd,
                                                Value = data.Substring(cvalueBegin, cvalueEnd - cvalueBegin),
                                                ValueBegin = cvalueBegin,
                                                ValueEnd = cvalueEnd,
                                                ConvertionType = convertionType
                                            });

                                            var elementEnd = LookForward(cvalueEnd, ELEMENT_END, ELEMENT_END.Length, ELEMENT_BEGIN);

                                            startIndex = elementEnd + 1;
                                        }
                                    }
                                }

                                var inputValue1Begin = LookForward(inputPrefixBegin, CKEY_INPUT_VALUE_1_IDENTIFIER, CKEY_INPUT_VALUE_1_IDENTIFIER.Length, ELEMENT_END);

                                if (inputPlaceHolder1Begin <= 0 && inputValue1Begin > 0)
                                {
                                    var inputValue2Begin = LookForward(inputValue1Begin, CKEY_INPUT_VALUE_2_IDENTIFIER, CKEY_INPUT_VALUE_2_IDENTIFIER.Length, ELEMENT_END);

                                    if (inputValue2Begin > 0)
                                    {
                                        var cvalueBegin = LookForward(inputValue2Begin, CKEY_INPUT_VALUE_3_IDENTIFIER, CKEY_INPUT_VALUE_3_IDENTIFIER.Length, ELEMENT_END);

                                        if (cvalueBegin > 0)
                                        {
                                            var cvalueEnd = LookForward(cvalueBegin, CKEY_INPUT_VALUE_3_IDENTIFIER, CKEY_INPUT_VALUE_3_IDENTIFIER.Length, ELEMENT_END) - 1;

                                            convertionType = ConvertionType.InputValue;

                                            ctranCoders.Add(new CtranCoder()
                                            {
                                                Key = data.Substring(ckeyBegin, ckeyEnd - ckeyBegin),
                                                KeyBegin = ckeyBegin,
                                                KeyEnd = ckeyEnd,
                                                ConvertionBegin = convertionBegin,
                                                ConvertionEnd = convertionEnd,
                                                Value = data.Substring(cvalueBegin, cvalueEnd - cvalueBegin),
                                                ValueBegin = cvalueBegin,
                                                ValueEnd = cvalueEnd,
                                                ConvertionType = convertionType
                                            });

                                            var elementEnd = LookForward(cvalueEnd, ELEMENT_END, ELEMENT_END.Length, ELEMENT_BEGIN);

                                            startIndex = elementEnd + 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // <a><tag key="KEY">VALUE</tag><br/></a>
                        // <a key="KEY">VALUE</a>
                        var tagBegin = LookBackward(ckeyPrefix1Begin, CKEY_TAG_1_IDENTIFIER, CKEY_TAG_1_IDENTIFIER.Length, ELEMENT_END);

                        if (tagBegin > 0)
                        {
                            convertionBegin = tagBegin;
                        }
                        else
                        {
                            convertionBegin = ckeyPrefix1Begin - CKEY_PREFIX_1_IDENTIFIER.Length + 1;
                        }

                        var ckeyPrefix2Begin = LookForward(ckeyPrefix1Begin, CKEY_PREFIX_2_IDENTIFIER, CKEY_PREFIX_2_IDENTIFIER.Length, ELEMENT_END);

                        if (ckeyPrefix2Begin > 0)
                        {
                            var ckeyBegin = LookForward(ckeyPrefix2Begin, CKEY_PREFIX_3_IDENTIFIER, CKEY_PREFIX_3_IDENTIFIER.Length, ELEMENT_END);

                            if (ckeyBegin > 0)
                            {
                                var ckeyEnd = LookForward(ckeyBegin, CKEY_SURFIX_1_IDENTIFIER, CKEY_SURFIX_1_IDENTIFIER.Length, ELEMENT_END) - 1;

                                if (ckeyEnd > ckeyBegin)
                                {
                                    convertionEnd = ckeyEnd + 1;

                                    var cvalueBegin = LookForward(ckeyEnd, CVALUE_PREFIX_1_IDENTIFIER, CVALUE_PREFIX_1_IDENTIFIER.Length, ELEMENT_BEGIN);

                                    if (cvalueBegin > 0)
                                    {
                                        var cvalueEnd = 0;

                                        if (tagBegin > 0)
                                        {
                                            cvalueEnd = LookForward(cvalueBegin, CKEY_TAG_END_1_IDENTIFIER, CKEY_TAG_END_1_IDENTIFIER.Length, CKEY_TAG_END_2_IDENTIFIER) - CKEY_TAG_END_1_IDENTIFIER.Length;
                                        }
                                        else
                                        {
                                            cvalueEnd = LookForward(cvalueBegin, CVALUE_SURFIX_1_IDENTIFIER, CVALUE_SURFIX_1_IDENTIFIER.Length, ELEMENT_END) - 1;
                                        }

                                        if (cvalueEnd > 0)
                                        {
                                            var tagPrefix = LookForward(cvalueEnd, ELEMENT_BEGIN, ELEMENT_BEGIN.Length, ELEMENT_END);

                                            if (tagPrefix > 0)
                                            {
                                                var tagEnd = LookForward(tagPrefix, CKEY_TAG_2_IDENTIFIER, CKEY_TAG_2_IDENTIFIER.Length, ELEMENT_BEGIN);

                                                if (tagEnd > 0)
                                                {
                                                    convertionEnd = tagEnd;

                                                    convertionType = ConvertionType.Tag;
                                                }
                                            }

                                            ctranCoders.Add(new CtranCoder()
                                            {
                                                Key = data.Substring(ckeyBegin, ckeyEnd - ckeyBegin),
                                                KeyBegin = ckeyBegin,
                                                KeyEnd = ckeyEnd,
                                                ConvertionBegin = convertionBegin,
                                                ConvertionEnd = convertionEnd,
                                                Value = data.Substring(cvalueBegin, cvalueEnd - cvalueBegin),
                                                ValueBegin = cvalueBegin,
                                                ValueEnd = cvalueEnd,
                                                ConvertionType = convertionType
                                            });

                                            startIndex = cvalueEnd + 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return startIndex;
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
    }
}
