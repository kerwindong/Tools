
using System;

using Newtonsoft.Json;

namespace BackBag.Business.App.Entities
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AppModel
    {
        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("version")]
        public decimal Version { set; get; }

        [JsonProperty("path")]
        public string Path { set; get; }

        [JsonProperty("startapp")]
        public string StartApp { set; get; }

        [JsonProperty("icon")]
        public string Icon { set; get; }

        public bool HasNew { set; get; }
    }
}
