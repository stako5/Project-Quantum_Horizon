using System;
using System.Collections.Generic;

namespace MMORPG.Client.Data
{
    [Serializable]
    public class AbilityEnvelope { public string @class; public List<AbilityModel> abilities; }

    [Serializable]
    public class AbilityModel
    {
        public string id;
        public string name;
        public string branch;
        public int tier;
        public string type;
        public string description;
        public float cooldown_s;
        public ResourceModel resource;
        public List<string> tags;
    }

    [Serializable]
    public class ResourceModel { public string type; public float cost; }
}

