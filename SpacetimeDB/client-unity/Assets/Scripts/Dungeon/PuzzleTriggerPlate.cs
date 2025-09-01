using UnityEngine;

namespace MMORPG.Client.Dungeon
{
    [RequireComponent(typeof(Collider))]
    public class PuzzleTriggerPlate : MonoBehaviour
    {
        public bool isActive { get; private set; }
        [SerializeField] private ParticleSystem vfxOn;
        [SerializeField] private Renderer plateRenderer;
        [SerializeField] private Color inactiveColor = new Color(0.2f,0.2f,0.2f);
        [SerializeField] private Color activeColor = new Color(0.0f,0.9f,0.6f);

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Activate();
        }

        public void Activate()
        {
            if (isActive) return;
            isActive = true;
            if (vfxOn) vfxOn.Play();
            if (plateRenderer && plateRenderer.material) plateRenderer.material.color = activeColor;
            var parent = GetComponentInParent<PuzzleRoomController>();
            if (parent) parent.NotifyPlateActivated(this);
        }

        public void Deactivate()
        {
            isActive = false;
            if (plateRenderer && plateRenderer.material) plateRenderer.material.color = inactiveColor;
        }
    }
}

