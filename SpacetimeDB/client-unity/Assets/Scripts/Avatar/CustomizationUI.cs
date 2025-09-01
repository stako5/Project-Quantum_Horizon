using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.Avatar
{
    public class CustomizationUI : MonoBehaviour
    {
        [Header("References")] [SerializeField] private AvatarApplier applier;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Slider weightSlider;
        [SerializeField] private Slider torsoSlider;
        [SerializeField] private Slider armSlider;
        [SerializeField] private Slider legSlider;

        private AvatarDefinition _def = new AvatarDefinition();

        void Start()
        {
            if (heightSlider) { heightSlider.minValue = 145; heightSlider.maxValue = 210; heightSlider.value = _def.height_cm; heightSlider.onValueChanged.AddListener(v => { _def.height_cm = Mathf.RoundToInt(v); Apply(); }); }
            if (weightSlider) { weightSlider.minValue = 35; weightSlider.maxValue = 180; weightSlider.value = _def.weight_kg; weightSlider.onValueChanged.AddListener(v => { _def.weight_kg = Mathf.RoundToInt(v); Apply(); }); }
            if (torsoSlider) { torsoSlider.minValue = 0.9f; torsoSlider.maxValue = 1.1f; torsoSlider.value = _def.body.torso_scale; torsoSlider.onValueChanged.AddListener(v => { _def.body.torso_scale = v; Apply(); }); }
            if (armSlider) { armSlider.minValue = 0.85f; armSlider.maxValue = 1.15f; armSlider.value = _def.body.arm_length_ratio; armSlider.onValueChanged.AddListener(v => { _def.body.arm_length_ratio = v; Apply(); }); }
            if (legSlider) { legSlider.minValue = 0.85f; legSlider.maxValue = 1.15f; legSlider.value = _def.body.leg_length_ratio; legSlider.onValueChanged.AddListener(v => { _def.body.leg_length_ratio = v; Apply(); }); }
            Apply();
        }

        public void Apply()
        {
            applier?.Apply(_def);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(_def);
        }
    }
}

