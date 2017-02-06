using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BackBag.Business.App.Entities
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class BackBagModel
    {
        [JsonProperty("installed")]
        public List<AppModel> InstalledApps { set; get; }

        [JsonProperty("uninstalled")]
        public List<AppModel> UninstalledApps { set; get; }
    }
}
