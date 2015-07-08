using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using StringUtility.Common.Text;
using StringUtility.Configuration;

namespace StringUtility.Utility
{
    public class DesUtility : IUtility
    {
        private const string NAME = "DES unicode";

        private const string MAIN_NAME = "Encryption";

        private const string ADVANCE_NAME = "Decryption";

        private const string KEY_NAME_LABEL = "Key";

        private const string DEFAULT_DECRYPTION_KEY = "fltonline";

        public DesUtility()
        {
            Name = NAME;

            MainName = MAIN_NAME;

            AdvanceName = ADVANCE_NAME;

            HasOtherInputs = true;

            OtherInputsText = KEY_NAME_LABEL;
        }

        public string Main(string str, params string[] args)
        {
            var encrypted = string.Empty;

            var encryptor = DEFAULT_DECRYPTION_KEY;

            if (ConfigManager.Get().DesConfig != null &&
                ConfigManager.Get().DesConfig.DesPolicy != null &&
                ConfigManager.Get().DesConfig.DesPolicy.Key.IsNullOrWhiteSpace() == false)
            {
                encryptor = ConfigManager.Get().DesConfig.DesPolicy.Key;
            }

            if (args != null && args.Length > 0 && args[0].IsNullOrWhiteSpace() == false)
            {
                encryptor = args[0];
            }

            if (str.IsNullOrWhiteSpace() == false)
            {
                var bytes = new PasswordDeriveBytes(encryptor, null).GetBytes(8);

                using (var provider = new DESCryptoServiceProvider())
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(stream, provider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write))
                        {
                            var buffer = Encoding.Unicode.GetBytes(str);

                            cryptoStream.Write(buffer, 0, buffer.Length);

                            cryptoStream.FlushFinalBlock();

                            encrypted = Convert.ToBase64String(stream.ToArray());
                        }
                    }
                }
            }

            return encrypted;
        }

        public string Advance(string str, params string[] args)
        {
            var decrypted = string.Empty;

            var encryptor = DEFAULT_DECRYPTION_KEY;

            if (ConfigManager.Get().DesConfig != null &&
                ConfigManager.Get().DesConfig.DesPolicy != null &&
                ConfigManager.Get().DesConfig.DesPolicy.Key.IsNullOrWhiteSpace() == false)
            {
                encryptor = ConfigManager.Get().DesConfig.DesPolicy.Key;
            }

            if (args != null && args.Length > 0 && args[0].IsNullOrWhiteSpace() == false)
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

        public string Name { set; get; }

        public string MainName { set; get; }

        public string AdvanceName { set; get; }

        public bool HasOtherInputs { set; get; }

        public string OtherInputsText { set; get; }
    }
}
