using System.Linq;
using UnityEngine;

namespace MMORPG.Client.Avatar
{
    public class AvatarApplier : MonoBehaviour
    {
        [Header("Rig References")]
        [SerializeField] private Transform spine;
        [SerializeField] private Transform leftUpperArm;
        [SerializeField] private Transform rightUpperArm;
        [SerializeField] private Transform leftUpperLeg;
        [SerializeField] private Transform rightUpperLeg;
        [SerializeField] private SkinnedMeshRenderer faceMesh;
        [SerializeField] private SkinnedMeshRenderer bodyMesh;

        [Header("Physics (optional)")]
        [SerializeField] private Rigidbody[] ragdollBodies;

        public void Apply(AvatarDefinition def)
        {
            // Apply overall scale based on height (assumes base height ~ 1.75m)
            float scale = Mathf.Clamp(def.height_cm / 175f, 145f/175f, 210f/175f);
            transform.localScale = new Vector3(scale, scale, scale);

            // Torso scale
            if (spine) spine.localScale = new Vector3(1f, def.body.torso_scale, 1f);

            // Limb proportional adjustments (very simplified)
            if (leftUpperArm) leftUpperArm.localScale = new Vector3(1f, def.body.arm_length_ratio, 1f);
            if (rightUpperArm) rightUpperArm.localScale = new Vector3(1f, def.body.arm_length_ratio, 1f);
            if (leftUpperLeg) leftUpperLeg.localScale = new Vector3(1f, def.body.leg_length_ratio, 1f);
            if (rightUpperLeg) rightUpperLeg.localScale = new Vector3(1f, def.body.leg_length_ratio, 1f);

            // Face blends
            if (faceMesh)
            {
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                {
                    var name = faceMesh.sharedMesh.GetBlendShapeName(i);
                    var found = def.face.blends.FirstOrDefault(b => b.name == name);
                    if (found != null)
                    {
                        faceMesh.SetBlendShapeWeight(i, Mathf.Clamp(found.weight, 0f, 100f));
                    }
                }
            }

            // Apply simple mass distribution (rough human model) if ragdoll available
            if (ragdollBodies != null && ragdollBodies.Length > 0)
            {
                float mass = Mathf.Clamp(def.weight_kg, 35, 180);
                // Distribute: torso 0.5, legs 0.3, arms 0.15, head 0.05
                foreach (var rb in ragdollBodies)
                {
                    if (!rb) continue;
                    string n = rb.name.ToLowerInvariant();
                    float f = n.Contains("head") ? 0.05f :
                              (n.Contains("spine") || n.Contains("chest") || n.Contains("hips")) ? 0.5f / 3f :
                              (n.Contains("thigh") || n.Contains("calf") || n.Contains("leg")) ? 0.3f / 4f :
                              (n.Contains("arm") || n.Contains("forearm")) ? 0.15f / 4f : 0.02f;
                    rb.mass = Mathf.Max(0.2f, mass * f);
                }
            }

            // TODO: Apply material colors for skin/hair/eyes on body/face materials
        }
    }
}

