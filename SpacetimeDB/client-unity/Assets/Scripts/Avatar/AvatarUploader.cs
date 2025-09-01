using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG.Client.Avatar
{
    public class AvatarUploader : MonoBehaviour
    {
        [SerializeField] private CustomizationUI customization;

        public async Task UploadAsync()
        {
            if (!customization) return;
            string json = customization.ToJson();
            Debug.Log($"[Avatar] Upload payload length={json.Length}");
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.ConnectAsync();
            await MMORPG.Client.Net.SpacetimeDbClient.Instance.CallReducerAsync("set_avatar", json);
        }
    }
}
