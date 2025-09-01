using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Avatar
{
    [Serializable]
    public class AvatarDefinition
    {
        public int height_cm = 175;
        public int weight_kg = 70;
        public BodyParams body = new BodyParams();
        public FaceParams face = new FaceParams();
    }

    [Serializable]
    public class BodyParams
    {
        [Range(0.9f, 1.1f)] public float torso_scale = 1.0f;
        [Range(0.85f, 1.15f)] public float arm_length_ratio = 1.0f;
        [Range(0.85f, 1.15f)] public float leg_length_ratio = 1.0f;
        public Color skin_color = new Color(0.8f, 0.7f, 0.6f);
        public Color hair_color = new Color(0.1f, 0.08f, 0.05f);
        public Color eye_color = new Color(0.2f, 0.5f, 0.7f);
        public string hair_style = "short";
    }

    [Serializable]
    public class FaceParams
    {
        // Blendshape weights 0..100 (names must match mesh blendshapes)
        public List<FaceBlend> blends = new List<FaceBlend>();
    }

    [Serializable]
    public class FaceBlend
    {
        public string name;
        [Range(0f, 100f)] public float weight;
    }
}

