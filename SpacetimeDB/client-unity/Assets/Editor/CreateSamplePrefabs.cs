using UnityEditor;
using UnityEngine;

public static class CreateSamplePrefabs
{
    [MenuItem("Tools/MMORPG/Create Sample Portal Prefabs")] 
    public static void CreatePortals()
    {
        EnsureFolders();
        CreateVoidRiftPortal();
        CreateArcologyShaftPortal();
        CreateWorldBossMarker();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Prefabs Created", "Created sample VoidRiftPortal and ArcologyShaftPortal in Assets/Prefabs/Portals.", "OK");
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets","Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Portals")) AssetDatabase.CreateFolder("Assets/Prefabs","Portals");
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets","Materials");
    }

    static void CreateVoidRiftPortal()
    {
        // Create a ring + particle fog
        var root = new GameObject("VoidRiftPortal");
        var ring = GameObject.CreatePrimitive(PrimitiveType.Torus);
#if UNITY_6000_0_OR_NEWER
        // Fallback if Torus primitive not available; build from cylinder
        if (!ring) ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
#endif
        ring.name = "Ring"; ring.transform.SetParent(root.transform, false); ring.transform.localScale = new Vector3(1.6f, 0.1f, 1.6f);
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", new Color(0.4f,0.05f,0.8f)*2f);
        var rend = ring.GetComponent<Renderer>(); if (rend) rend.sharedMaterial = mat;
        var ctrl = root.AddComponent<MMORPG.Client.Portals.PortalVFXController>();
        ctrl.GetType().GetField("emissiveRenderer", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance)?.SetValue(ctrl, rend);

        var psGo = new GameObject("VoidFog"); psGo.transform.SetParent(root.transform, false);
        var ps = psGo.AddComponent<ParticleSystem>(); var main = ps.main; main.startColor = new Color(0.3f,0.0f,0.5f,0.5f); main.startSize = 0.6f; main.startLifetime = 2.5f; var emission = ps.emission; emission.rateOverTime = 40f;

        var prefabPath = "Assets/Prefabs/Portals/VoidRiftPortal.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        GameObject.DestroyImmediate(root);
    }

    static void CreateArcologyShaftPortal()
    {
        var root = new GameObject("ArcologyShaftPortal");
        var frame = GameObject.CreatePrimitive(PrimitiveType.Cube); frame.name = "Frame"; frame.transform.SetParent(root.transform, false); frame.transform.localScale = new Vector3(1.6f, 2.2f, 0.2f);
        var glass = GameObject.CreatePrimitive(PrimitiveType.Plane); glass.name = "Glass"; glass.transform.SetParent(root.transform, false); glass.transform.localScale = new Vector3(0.12f,1f,0.12f); glass.transform.localPosition = new Vector3(0,0,0.1f);
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); mat.SetFloat("_Surface", 1); mat.SetColor("_BaseColor", new Color(0.1f,0.7f,0.9f,0.2f)); mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", new Color(0.0f,0.5f,1.0f));
        var rend = glass.GetComponent<Renderer>(); if (rend) rend.sharedMaterial = mat;

        var water = new GameObject("Mist"); water.transform.SetParent(root.transform, false); var ps = water.AddComponent<ParticleSystem>(); var main = ps.main; main.startColor = new Color(0.2f,0.8f,1f,0.4f); main.startSize = 0.3f; main.startLifetime = 1.5f; var emission = ps.emission; emission.rateOverTime = 60f;

        var prefabPath = "Assets/Prefabs/Portals/ArcologyShaftPortal.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        GameObject.DestroyImmediate(root);
    }

    static void CreateWorldBossMarker()
    {
        var root = new GameObject("WorldBossMarker");
        var billboard = GameObject.CreatePrimitive(PrimitiveType.Quad);
        billboard.transform.SetParent(root.transform, false);
        billboard.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.EnableKeyword("_EMISSION"); mat.SetColor("_BaseColor", new Color(1f,0.5f,0f,1f));
        var rend = billboard.GetComponent<Renderer>(); rend.sharedMaterial = mat;
        root.AddComponent<UnityEngine.BillboardRenderer>();
        var path = "Assets/Prefabs/Portals/WorldBossMarker.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
    }
}
