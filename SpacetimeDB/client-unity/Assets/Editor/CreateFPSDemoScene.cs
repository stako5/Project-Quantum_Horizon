using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMORPG.Client.EditorTools
{
    public static class CreateFPSDemoScene
    {
        [MenuItem("MMORPG/Player/Create FPS Demo Scene")] 
        public static void CreateScene()
        {
            EnsureScenesFolder();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            ground.GetComponent<Renderer>().sharedMaterial.color = new Color(0.22f, 0.25f, 0.28f);

            // Player
            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1.2f, 0f);
            player.AddComponent<CharacterController>();
            // Core systems
            var health = player.AddComponent<MMORPG.Client.Combat.Health>();
            player.AddComponent<MMORPG.Client.Combat.BuffManager>();
            player.AddComponent<MMORPG.Client.Items.Inventory>();
            player.AddComponent<MMORPG.Client.Items.PickupCollector>();
            // FPS controller
            player.AddComponent<MMORPG.Client.Player.FirstPersonController>();
            // Head + camera
            var head = new GameObject("Head"); head.transform.SetParent(player.transform, false); head.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            head.AddComponent<MMORPG.Client.Player.FPSCamera>();
            // Weapon hitbox (simple forward capsule trigger)
            var weapon = GameObject.CreatePrimitive(PrimitiveType.Capsule); weapon.name = "WeaponHitbox";
            weapon.transform.SetParent(player.transform, false); weapon.transform.localPosition = new Vector3(0f, 1.2f, 0.8f); weapon.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            var wcol = weapon.GetComponent<Collider>(); wcol.isTrigger = true;
            var mhd = weapon.AddComponent<MMORPG.Client.Combat.MeleeHitboxDamage>(); mhd.SetOwner(player.transform, player.GetComponent<MMORPG.Client.Combat.BuffManager>(), health);

            // Audio SFX manager
            new GameObject("SfxPlayer").AddComponent<MMORPG.Client.Audio.SfxPlayer>();
            // Optimizer
            new GameObject("PlatformOptimizer").AddComponent<MMORPG.Client.Boot.PlatformOptimizer>();

            // UI Canvas
            var canvasGo = CreateCanvas(out var canvas);
            EnsureEventSystem();

            // Crosshair (center)
            var cross = new GameObject("Crosshair"); cross.transform.SetParent(canvasGo.transform, false);
            var crt = cross.AddComponent<RectTransform>(); crt.anchorMin = new Vector2(0.5f, 0.5f); crt.anchorMax = new Vector2(0.5f, 0.5f); crt.sizeDelta = new Vector2(6, 6);
            var cimg = cross.AddComponent<Image>(); cimg.color = new Color(1f, 1f, 1f, 0.8f);

            // Options Panel (top-right)
            var options = new GameObject("OptionsPanel"); options.transform.SetParent(canvasGo.transform, false);
            var ort = options.AddComponent<RectTransform>(); ort.anchorMin = new Vector2(1, 1); ort.anchorMax = new Vector2(1, 1); ort.pivot = new Vector2(1, 1); ort.anchoredPosition = new Vector2(-24, -24); ort.sizeDelta = new Vector2(340, 420);
            var opbg = options.AddComponent<Image>(); opbg.color = new Color(0, 0, 0, 0.35f);
            var vlg = options.AddComponent<VerticalLayoutGroup>(); vlg.childControlHeight = true; vlg.childForceExpandHeight = false; vlg.spacing = 6;

            // Headings
            Text Header(string txt)
            {
                var go = new GameObject("Header"); go.transform.SetParent(options.transform, false);
                var t = go.AddComponent<Text>(); t.text = txt; t.fontSize = 16; t.color = Color.white; t.alignment = TextAnchor.MiddleCenter; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 26);
                return t;
            }

            // Quality Dropdown
            Header("Quality");
            var qRow = new GameObject("QualityRow"); qRow.transform.SetParent(options.transform, false);
            var qrt = qRow.AddComponent<RectTransform>(); qrt.sizeDelta = new Vector2(0, 28);
            var qbg = qRow.AddComponent<Image>(); qbg.color = new Color(0, 0, 0, 0.25f);
            var qdd = qRow.AddComponent<Dropdown>(); qdd.options.Add(new Dropdown.OptionData("Quality"));
            qRow.AddComponent<MMORPG.Client.UI.QualityDropdown>();

            // Display Options
            Header("Display");
            // Resolution dropdown
            var resRow = MakeRow(options.transform, "Resolution");
            var resDd = resRow.AddComponent<Dropdown>();
            // Fullscreen toggle
            var fsRow = MakeRow(options.transform, "Fullscreen");
            var fsToggle = fsRow.AddComponent<Toggle>(); fsRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);
            // VSync toggle
            var vsRow = MakeRow(options.transform, "VSync");
            var vsToggle = vsRow.AddComponent<Toggle>(); vsRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);

            var display = options.AddComponent<MMORPG.Client.UI.DisplayOptions>();
            // assign via reflection (private serialized fields)
            Assign(display, "resolutionDropdown", resDd);
            Assign(display, "fullscreenToggle", fsToggle);
            Assign(display, "vsyncToggle", vsToggle);

            // FPS Settings UI
            Header("FPS Settings");
            var sensX = MakeSlider(options.transform, "Sensitivity X", 0.5f, 10f, 2.4f);
            var sensY = MakeSlider(options.transform, "Sensitivity Y", 0.5f, 10f, 2.0f);
            var fov = MakeSlider(options.transform, "FOV", 50f, 110f, 70f);
            var inv = MakeToggle(options.transform, "Invert Y", false);
            var walk = MakeSlider(options.transform, "Walk Speed", 1f, 10f, 4.2f);
            var sprint = MakeSlider(options.transform, "Sprint Speed", 2f, 14f, 6.8f);

            var fpsui = options.AddComponent<MMORPG.Client.UI.FPSSettingsUI>();
            Assign(fpsui, "sensitivityX", sensX);
            Assign(fpsui, "sensitivityY", sensY);
            Assign(fpsui, "invertY", inv);
            Assign(fpsui, "fov", fov);
            Assign(fpsui, "walkSpeed", walk);
            Assign(fpsui, "sprintSpeed", sprint);

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/FPSDemo.unity");
            EditorUtility.DisplayDialog("FPS Demo Created", "Generated FPSDemo scene in Assets/Scenes with player, FPS controls, and options panel.", "OK");
        }

        static GameObject CreateCanvas(out Canvas canvas)
        {
            var go = new GameObject("Canvas");
            canvas = go.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>(); go.AddComponent<GraphicRaycaster>();
            return go;
        }

        static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        static void EnsureEventSystem()
        {
            if (!Object.FindObjectOfType<EventSystem>())
            {
                var es = new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>();
            }
        }

        static GameObject MakeRow(Transform parent, string label)
        {
            var row = new GameObject(label + "Row"); row.transform.SetParent(parent, false);
            var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28);
            var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.25f);
            var textGo = new GameObject("Label"); textGo.transform.SetParent(row.transform, false);
            var trt = textGo.AddComponent<RectTransform>(); trt.anchorMin = new Vector2(0,0); trt.anchorMax = new Vector2(0.5f,1); trt.offsetMin = new Vector2(6,2); trt.offsetMax = new Vector2(-6,-2);
            var t = textGo.AddComponent<Text>(); t.text = label; t.color = Color.white; t.alignment = TextAnchor.MiddleLeft; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            var ctl = new GameObject("Control"); ctl.transform.SetParent(row.transform, false);
            var crt = ctl.AddComponent<RectTransform>(); crt.anchorMin = new Vector2(0.5f,0); crt.anchorMax = new Vector2(1,1); crt.offsetMin = new Vector2(6,2); crt.offsetMax = new Vector2(-6,-2);
            return ctl;
        }

        static Slider MakeSlider(Transform parent, string label, float min, float max, float value)
        {
            var ctl = MakeRow(parent, label);
            var slider = ctl.AddComponent<Slider>();
            slider.minValue = min; slider.maxValue = max; slider.value = value;
            return slider;
        }

        static Toggle MakeToggle(Transform parent, string label, bool value)
        {
            var ctl = MakeRow(parent, label);
            var toggle = ctl.AddComponent<Toggle>(); toggle.isOn = value;
            return toggle;
        }

        static void Assign(object comp, string fieldName, object value)
        {
            var f = comp.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) f.SetValue(comp, value);
        }
    }
}

