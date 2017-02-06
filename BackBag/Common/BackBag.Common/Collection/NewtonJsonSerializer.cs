using System;
using System.Text;

using BackBag.Common.Log;

using Newtonsoft.Json;

namespace BackBag.Common.Collection
{
    public class NewtonJsonSerializer 
    {
        private NewtonJsonSerializer()
        {

        }

        public byte[] SerializeToBytes<T>(T t)
        {
            if (t != null)
            {
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(t));
            }

            return default(byte[]);
        }

        public string Serialize<T>(T t, bool indented = false)
        {
            if (t != null)
            {
                try
                {
                    if (indented)
                    {
                        return JsonConvert.SerializeObject(t, Formatting.Indented);
                    }

                    return JsonConvert.SerializeObject(t);
                }
                catch (Exception ex)
                {
                    FileLogger.Instance.Log(ex);

                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public T Deserialize<T>(string serializedString)
        {
            if (!string.IsNullOrWhiteSpace(serializedString))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(serializedString);
                }
                catch (Exception ex)
                {
                    FileLogger.Instance.Log(ex);

                    return default(T);
                }
            }

            return default(T);
        }

        public T Deserialize<T>(byte[] serializedBytes)
        {
            if (serializedBytes != null && serializedBytes.Length > 0)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serializedBytes));
            }

            return default(T);
        }

        #region Singleton

        public static NewtonJsonSerializer Instance { get { return InternalNewtonJsonSerializer.Instance; } }

        private class InternalNewtonJsonSerializer
        {
            // Tell C# compiler not to mark type as beforefieldinit
            static InternalNewtonJsonSerializer()
            {
            }

            internal static readonly NewtonJsonSerializer Instance = new NewtonJsonSerializer();
        }

        #endregion
    }
}
