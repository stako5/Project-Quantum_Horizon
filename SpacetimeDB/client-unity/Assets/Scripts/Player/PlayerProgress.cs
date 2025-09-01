using UnityEngine;

namespace MMORPG.Client.Player
{
    public class PlayerProgress : MonoBehaviour
    {
        public int level = 1;
        public long xp = 0;
        public long xpToNext = 100;

        public System.Action<int> OnLevelUp;
        public System.Action<long,long> OnXPChanged; // xp, toNext

        public void AddXP(long amount)
        {
            xp += Mathf.Max(0, (int)amount);
            while (xp >= xpToNext)
            {
                xp -= xpToNext; level++;
                xpToNext = NextThreshold(level, xpToNext);
                OnLevelUp?.Invoke(level);
            }
            OnXPChanged?.Invoke(xp, xpToNext);
        }

        static long NextThreshold(int level, long prev)
        {
            // Simple exponential curve
            return Mathf.RoundToInt(prev * 1.25f + 25f * Mathf.Pow(level, 1.1f));
        }
    }
}

