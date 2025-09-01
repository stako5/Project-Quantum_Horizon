using System.Collections.Generic;
using MMORPG.Client.Combat;

namespace MMORPG.Client.Items
{
    // Typed consumable used across UI/consumption layers
    public class Consumable
    {
        public string Id;
        public string Name;
        public int Tier;
        public string Category;
        public float CooldownS;
        public BuffStackMode StackMode;
        public List<BuffEffect> Effects = new();
    }
}

