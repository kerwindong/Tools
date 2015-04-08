using System;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;

namespace StringUtility.Configuration
{
    public static class ConfigManager
    {
        public static ConfigurationFileManager Get()
        {
            return ConfigurationFileManager.Instance;
        }
    }

    public sealed class ConfigurationFileManager
    {
        private const string MAPPER_CONFIG = "MapperConfig";

        private const string CTRAN_CONFIG = "CtranConfig";

        private string BaseDirectory = string.Empty;

        private static readonly object locker = new object();

        private ConfigurationFileManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            lock (locker)
            {
                MapperConfig = Load<MapperConfig>(MAPPER_CONFIG);

                CtranConfig = Load<CtranConfig>(CTRAN_CONFIG);
            }
        }

        public MapperConfig MapperConfig { set; get; }

        public CtranConfig CtranConfig { set; get; }

        private T Load<T>(string setting)
        {
            T c = default(T);

            var fileName = ConfigurationManager.AppSettings[setting];
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var filePath = string.Concat(BaseDirectory, fileName);
                var serializer = new XmlSerializer(typeof(T));
                using (var reader = new StreamReader(filePath))
                {
                    c = (T)serializer.Deserialize(reader);
                    reader.Close();
                }
            }

            return c;
        }

        #region Singleton

        public static ConfigurationFileManager Instance { get { return InternalConfigurationFileManager.Instance; } }

        private class InternalConfigurationFileManager
        {
            // Tell C# compiler not to mark type as beforefieldinit
            static InternalConfigurationFileManager()
            {
            }

            internal static readonly ConfigurationFileManager Instance = new ConfigurationFileManager();
        }

        #endregion
    }
}
