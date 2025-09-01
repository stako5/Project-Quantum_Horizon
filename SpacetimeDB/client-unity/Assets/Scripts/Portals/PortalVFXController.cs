using UnityEngine;

namespace MMORPG.Client.Portals
{
    public class PortalVFXController : MonoBehaviour
    {
        [SerializeField] private Renderer emissiveRenderer;
        [SerializeField] private Color emissiveColor = new Color(0.6f, 0.1f, 0.9f);
        [SerializeField] private float pulseSpeed = 2.5f;
        [SerializeField] private float rotationSpeed = 30f;

        void Update()
        {
            if (emissiveRenderer && emissiveRenderer.material)
            {
                float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * 2.0f + 0.2f;
                emissiveRenderer.material.SetColor("_EmissionColor", emissiveColor * pulse);
            }
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}

