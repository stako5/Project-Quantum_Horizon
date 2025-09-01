using System;
using System.Collections.Generic;

namespace MMORPG.Client.Data
{
    [Serializable]
    public class PerkEnvelope { public string @class; public List<PerkModel> perks; }

    [Serializable]
    public class PerkModel
    {
        public string id;
        public string name;
        public string tier;
        public List<string> tags;
        public string description;
        public List<PerkRoll> rolls;
    }

    [Serializable]
    public class PerkRoll { public string stat; public float min; public float max; }
}

