using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BackBag.Business.App.Entities
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AppHostModel
    {
        [JsonProperty("apphost")]
        public string AppHost { set; get; }

        [JsonProperty("apphostfile")]
        public string AppHostFile { set; get; }

        [JsonProperty("rootapp")]
        public AppModel RootApp { set; get; }

        [JsonProperty("apps")]
        public List<AppModel> Apps { set; get; }
    }
}
