using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Avatar
{
    public class FaceScanController : MonoBehaviour
    {
        [SerializeField] private Renderer previewRenderer;
        [SerializeField] private int width = 512;
        [SerializeField] private int height = 512;

        private WebCamTexture _webcam;

        public void StartCamera()
        {
            if (_webcam != null) return;
            _webcam = new WebCamTexture(width, height, 30);
            _webcam.Play();
            if (previewRenderer)
            {
                var mat = previewRenderer.material;
                mat.mainTexture = _webcam;
            }
        }

        public Texture2D CaptureFace()
        {
            if (_webcam == null || !_webcam.isPlaying) return null;
            var tex = new Texture2D(_webcam.width, _webcam.height, TextureFormat.RGBA32, false);
            tex.SetPixels32(_webcam.GetPixels32());
            tex.Apply();
            return tex;
        }

        public void StopCamera()
        {
            if (_webcam != null) { _webcam.Stop(); _webcam = null; }
        }

        public Dictionary<string, float> ReconstructFaceBlends(Texture2D captured)
        {
            // Placeholder: this would call into an AR/ML plugin or service that converts an image into blendshape weights.
            // For now, return an empty dictionary (identity face); integrate ARKit/ARCore/ReadyPlayerMe as desired.
            return new Dictionary<string, float>();
        }
    }
}

