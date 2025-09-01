using UnityEngine;

namespace MMORPG.Client.UI
{
    public class BestiaryPreviewRenderer : MonoBehaviour
    {
        [SerializeField] private Camera previewCamera;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private Transform stageRoot;
        [SerializeField] private Light keyLight;
        [SerializeField] private float rotationSpeed = 15f;

        private GameObject _current;

        public RenderTexture Texture => renderTexture;

        void Awake()
        {
            if (!stageRoot)
            {
                var go = new GameObject("StageRoot"); go.transform.SetParent(transform, false); stageRoot = go.transform;
            }
            if (!previewCamera)
            {
                var camGo = new GameObject("PreviewCamera"); camGo.transform.SetParent(transform, false);
                previewCamera = camGo.AddComponent<Camera>(); previewCamera.clearFlags = CameraClearFlags.Color; previewCamera.backgroundColor = Color.black;
            }
            if (!renderTexture)
            {
                renderTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
            }
            previewCamera.targetTexture = renderTexture;
            if (!keyLight)
            {
                var lgo = new GameObject("KeyLight"); lgo.transform.SetParent(transform, false); keyLight = lgo.AddComponent<Light>(); keyLight.type = LightType.Directional; keyLight.intensity = 1.2f; keyLight.transform.eulerAngles = new Vector3(45,135,0);
            }
        }

        void Update()
        {
            if (_current) _current.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        public void Render(GameObject prefab)
        {
            if (_current) Destroy(_current);
            if (!prefab)
            {
                // placeholder
                _current = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                _current.transform.SetParent(stageRoot, false);
            }
            else
            {
                _current = Instantiate(prefab, stageRoot);
            }
            // Position model and camera
            var bounds = CalculateBounds(_current);
            var radius = Mathf.Max(bounds.extents.x, bounds.extents.z, 0.5f);
            var center = bounds.center;
            _current.transform.position = -center; // center it at origin
            var dist = radius * 3.0f;
            previewCamera.transform.position = new Vector3(0, radius * 1.2f, dist);
            previewCamera.transform.LookAt(Vector3.zero);
        }

        static Bounds CalculateBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);
            var b = renderers[0].bounds;
            for (int i=1;i<renderers.Length;i++) b.Encapsulate(renderers[i].bounds);
            // convert to local
            b.center = go.transform.InverseTransformPoint(b.center);
            return b;
        }
    }
}

