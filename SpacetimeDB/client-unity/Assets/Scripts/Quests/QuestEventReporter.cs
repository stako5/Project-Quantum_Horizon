using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Quests
{
    public static class QuestEventReporter
    {
        public static async Task ReportAsync(string eventType, string param = "", uint count = 1)
        {
            try
            {
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
                await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("update_quest_on_event", eventType, param ?? string.Empty, count);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[QuestEvent] report failed: {e.Message}");
            }
        }
    }
}
