using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

namespace MMORPG.Client.World
{
    // Attempts to build a NavMesh at runtime using NavMeshSurface if present.
    public class NavMeshBootstrap : MonoBehaviour
    {
        [Tooltip("Rebuild at Start if the scene has no baked NavMesh.")]
        public bool buildOnStart = true;
        [Tooltip("If true, always rebuild regardless of existing NavMesh.")]
        public bool forceRebuild = false;

        void Start()
        {
            if (!buildOnStart) return;
            TryBuild();
        }

        public void TryBuild()
        {
            if (!forceRebuild)
            {
                // Heuristic: if any NavMesh exists near origin, skip
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Vector3.zero, out hit, 500f, NavMesh.AllAreas)) return;
            }

            // Look for NavMeshSurface (AI Navigation package). If not present, log tip.
            var type = Type.GetType("Unity.AI.Navigation.NavMeshSurface, Unity.AI.Navigation");
            if (type == null)
            {
                type = Type.GetType("NavMeshSurface"); // fallback for older namespaces
            }
            if (type != null)
            {
                var go = new GameObject("RuntimeNavMesh");
                go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                var comp = go.AddComponent(type);
                // Call BuildNavMesh via reflection
                var mi = type.GetMethod("BuildNavMesh", BindingFlags.Public | BindingFlags.Instance);
                mi?.Invoke(comp, null);
                Debug.Log("[NavMeshBootstrap] Built runtime NavMesh via NavMeshSurface.");
            }
            else
            {
                Debug.LogWarning("[NavMeshBootstrap] NavMeshSurface not found. Install 'AI Navigation' package or bake NavMesh offline.");
            }
        }
    }
}

