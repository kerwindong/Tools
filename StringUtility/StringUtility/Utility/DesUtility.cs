using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StringUtility.Utility
{
    public class DesUtility : IUtility
    {
        private const string NAME = "DES unicode";

        private const string MAIN_NAME = "Decryption";

        private const string ADVANCE_NAME = "";

        private const string DECRYPTION_KEY = "DecryptionKey";

        private const string DEFAULT_DECRYPTION_KEY = "fltonline";

        public DesUtility()
        {
            Name = NAME;

            MainName = MAIN_NAME;

            AdvanceName = ADVANCE_NAME;

            HasOtherInputs = true;

            OtherInputsText = DECRYPTION_KEY;
        }

        public string Main(string str, params string[] args)
        {
            var decrypted = string.Empty;

            var encryptor = DEFAULT_DECRYPTION_KEY;

            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                encryptor = args[0];
            }

            if (!string.IsNullOrWhiteSpace(str))
            {
                using (var provider = new DESCryptoServiceProvider())
                {
                    var bytes = new PasswordDeriveBytes(encryptor, null).GetBytes(8);

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, provider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Write))
                        {
                            var encrypted = Convert.FromBase64String(str);
                            cryptoStream.Write(encrypted, 0, encrypted.Length);

                            cryptoStream.FlushFinalBlock();

                            decrypted = Encoding.Unicode.GetString(memoryStream.ToArray());
                        }
                    }
                }
            }

            return decrypted;
        }

        public string Advance(string str)
        {
            return string.Empty;
        }

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public bool HasOtherInputs { set; get; }

        public string OtherInputsText { set; get; }
    }
}
