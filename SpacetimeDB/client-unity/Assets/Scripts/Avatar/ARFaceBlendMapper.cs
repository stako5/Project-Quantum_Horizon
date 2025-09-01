using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Avatar
{
    public class ARFaceBlendMapper : MonoBehaviour
    {
        [SerializeField] private AvatarApplier applier;
        [SerializeField] private AvatarDefinition target = new AvatarDefinition();

#if ARFOUNDATION_PRESENT && UNITY_IOS
        UnityEngine.XR.ARFoundation.ARFaceManager _faceManager;
        void Awake() { _faceManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARFaceManager>(); }
        void OnEnable()
        {
            if (_faceManager != null)
                _faceManager.facesChanged += OnFacesChanged;
        }
        void OnDisable()
        {
            if (_faceManager != null)
                _faceManager.facesChanged -= OnFacesChanged;
        }

        void OnFacesChanged(UnityEngine.XR.ARFoundation.ARFacesChangedEventArgs args)
        {
            if (_faceManager.trackables.count == 0) return;
            var face = _faceManager.trackables[0];
            // Get ARKit coefficients
            var subsystem = (UnityEngine.XR.ARKit.ARKitFaceSubsystem)_faceManager.subsystem;
            using (var coeffs = subsystem.GetBlendShapeCoefficients(face.trackableId, Unity.Collections.Allocator.Temp))
            {
                target.face.blends.Clear();
                foreach (var c in coeffs)
                {
                    target.face.blends.Add(new FaceBlend { name = c.blendShapeLocation.ToString(), weight = c.coefficient * 100f });
                }
                applier?.Apply(target);
            }
        }
#else
        void Start() { Debug.Log("[ARFaceBlendMapper] ARFoundation not present or not on iOS; mapper disabled."); }
#endif
    }
}

