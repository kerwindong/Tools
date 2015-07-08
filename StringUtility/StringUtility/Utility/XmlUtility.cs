using System.Text;

namespace StringUtility
{
	public class XmlUtility : IUtility
	{
		private static readonly char[] s_escapeChars = new char[] { '<', '>', '"', '\'', '&' };
		private static readonly string[] s_escapeStringPairs = new string[] { "<", "&lt;", ">", "&gt;", "\"", "&quot;", "'", "&apos;", "&", "&amp;" };
		private static readonly char s_unMainCharPrefix = '&';
		private static readonly char s_unMainCharSurfix = ';';
		private static readonly char[] s_unescapeCharPairsTwo = new char[] { 'l', 't', '<', 'g', 't', '>' };
		private static readonly char[] s_unescapeCharPairsThree = new char[] { 'a', 'm', 'p', '&' };
		private static readonly char[] s_unescapeCharPairsFour = new char[] { 'q', 'u', 'o', 't', '"', 'a', 'p', 'o', 's', '\'' };

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

        public XmlUtility() {

            Name = "Xml";

            MainName = "Escape";

            AdvanceName = "Unescape";
        }

        public string Main(string str, params string[] args)
		{
			if (str == null)
			{
				return null;
			}
			StringBuilder builder = null;
			int length = str.Length;
			int startIndex = 0;
			while (true)
			{
				int num2 = str.IndexOfAny(s_escapeChars, startIndex);
				if (num2 == -1)
				{
					if (builder == null)
					{
						return str;
					}
					builder.Append(str, startIndex, length - startIndex);
					return builder.ToString();
				}
				if (builder == null)
				{
					builder = new StringBuilder();
				}
				builder.Append(str, startIndex, num2 - startIndex);
				builder.Append(GetMainSequence(str[num2]));
				startIndex = num2 + 1;
			}
		}

		private static string GetMainSequence(char c)
		{
			int length = s_escapeStringPairs.Length;
			for (int i = 0; i < length; i += 2)
			{
				string str = s_escapeStringPairs[i];
				string str2 = s_escapeStringPairs[i + 1];
				if (str[0] == c)
				{
					return str2;
				}
			}
			return c.ToString();
		}

        public string Advance(string str, params string[] args)
		{
			if (str == null)
			{
				return null;
			}
			StringBuilder builder = null;
			int length = str.Length;
			int startIndex = 0;
			while (true)
			{
				int num2 = str.IndexOf(s_unMainCharPrefix, startIndex);
				if (num2 == -1)
				{
					if (builder == null)
					{
						return str;
					}
					builder.Append(str, startIndex, length - startIndex);
					return builder.ToString();
				}
				if (builder == null)
				{
					builder = new StringBuilder();
				}
				builder.Append(str, startIndex, num2 - startIndex);
				if (length - num2 >= 4 && str[num2 + 3] == s_unMainCharSurfix)
				{
					builder.Append(GetUnMainSequence2(new char[] { str[num2 + 1], str[num2 + 2] }));
					startIndex = num2 + 4;
				}
				if (length - num2 >= 5 && str[num2 + 4] == s_unMainCharSurfix)
				{
					builder.Append(GetUnMainSequence3(new char[] { str[num2 + 1], str[num2 + 2], str[num2 + 3] }));
					startIndex = num2 + 5;
				}
				if (length - num2 >= 6 && str[num2 + 5] == s_unMainCharSurfix)
				{
					builder.Append(GetUnMainSequence4(new char[] { str[num2 + 1], str[num2 + 2], str[num2 + 3], str[num2 + 4] }));
					startIndex = num2 + 6;
				}
			}
		}

		private static char GetUnMainSequence2(char[] cs)
		{
			int length = s_unescapeCharPairsTwo.Length;
			for (int i = 0; i < length; i += 3)
			{
				bool ismatch = true;
				for (int j = 0; j < 2; j++) {
					if (cs[j] != s_unescapeCharPairsTwo[i + j]) {
						ismatch = false;
						break;
					}
				}
				if (ismatch) {
					return s_unescapeCharPairsTwo[i + 2];
				}
			}
			return new char();
		}

		private static char GetUnMainSequence3(char[] cs)
		{
            int length = s_unescapeCharPairsThree.Length;
			for (int i = 0; i < length; i += 4)
			{
				bool ismatch = true;
				for (int j = 0; j < 3; j++)
				{
                    if (cs[j] != s_unescapeCharPairsThree[i + j])
					{
						ismatch = false;
						break;
					}
				}
				if (ismatch)
				{
                    return s_unescapeCharPairsThree[i + 3];
				}
			}
			return new char();
		}

		private static char GetUnMainSequence4(char[] cs)
		{
			int length = s_unescapeCharPairsFour.Length;
			for (int i = 0; i < length; i += 5)
			{
				bool ismatch = true;
				for (int j = 0; j < 4; j++)
				{
                    if (cs[j] != s_unescapeCharPairsFour[i + j])
					{
						ismatch = false;
						break;
					}
				}
				if (ismatch)
				{
                    return s_unescapeCharPairsFour[i + 4];
				}
			}
			return new char();
		}


        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }
    }
}
