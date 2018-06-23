using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PostUserActivity.Contracts.Network
{
    public enum ArmDataPackageType
    {
        [JsonProperty("fullframe")]
        FullFrame,
        [JsonProperty("preview")]
        Preview,
        [JsonProperty("log")]
        Log,
    }
}
