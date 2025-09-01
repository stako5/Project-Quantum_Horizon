using System;
using System.Collections.Generic;

namespace MMORPG.Client.Enemies
{
    [Serializable]
    public class EnemyEnvelope { public int count; public List<EnemyTypeModel> enemies; }

    [Serializable]
    public class EnemyTypeModel
    {
        public string id;
        public string name;
        public string family;
        public int tier;
        public string role;
        public string element;
        public string size;
        public string movement;
        public string armor;
        public int hp;
        public int damage;
        public float speed;
        public List<string> abilities;
        public List<string> tags;
        public string description;
    }
}

