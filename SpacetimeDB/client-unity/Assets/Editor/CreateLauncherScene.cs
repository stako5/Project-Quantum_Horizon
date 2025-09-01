using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMORPG.Client.EditorTools
{
    public static class CreateLauncherScene
    {
        [MenuItem("MMORPG/Launcher/Create Launcher Scene")] 
        public static void CreateScene()
        {
            EnsureScenesFolder();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            // Canvas + EventSystem
            var canvasGo = CreateCanvas(out var canvas);
            EnsureEventSystem();

            // Background panel
            var bg = new GameObject("Background"); bg.transform.SetParent(canvasGo.transform, false);
            var bgRt = bg.AddComponent<RectTransform>(); bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
            var bgImg = bg.AddComponent<Image>(); bgImg.color = new Color(0.05f, 0.05f, 0.07f, 1f);

            // Logo + Title
            var title = new GameObject("Title"); title.transform.SetParent(bg.transform, false);
            var trt = title.AddComponent<RectTransform>(); trt.anchorMin = new Vector2(0.5f, 1f); trt.anchorMax = new Vector2(0.5f, 1f); trt.pivot = new Vector2(0.5f, 1f); trt.anchoredPosition = new Vector2(0, -40); trt.sizeDelta = new Vector2(800, 64);
            var tText = title.AddComponent<Text>(); tText.text = "title"; tText.alignment = TextAnchor.MiddleCenter; tText.fontSize = 36; tText.color = Color.white; tText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            title.AddComponent<MMORPG.Client.Localization.Localizer>();

            // Left column (scene + play)
            var left = new GameObject("Left"); left.transform.SetParent(bg.transform, false);
            var lrt = left.AddComponent<RectTransform>(); lrt.anchorMin = new Vector2(0f, 0f); lrt.anchorMax = new Vector2(0.5f, 1f); lrt.offsetMin = new Vector2(24, 24); lrt.offsetMax = new Vector2(-12, -24);
            var llg = left.AddComponent<VerticalLayoutGroup>(); llg.spacing = 8; llg.childControlHeight = true; llg.childForceExpandHeight = false;

            // Scene dropdown
            var sceneRow = MakeRow(left.transform, "Start Scene");
            var sceneDd = sceneRow.AddComponent<Dropdown>();

            // Profile dropdown
            MakeHeader(left.transform, "Profile");
            var profRow = MakeRow(left.transform, "Select Profile");
            var profDd = profRow.AddComponent<Dropdown>();
            var newRow = MakeRow(left.transform, "New Profile");
            var newInput = newRow.AddComponent<InputField>(); newRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);
            var createBtn = MakeButton(left.transform, "Create Profile");

            // Play/Quit buttons
            var btnRow = new GameObject("Buttons"); btnRow.transform.SetParent(left.transform, false);
            var brt = btnRow.AddComponent<RectTransform>(); brt.sizeDelta = new Vector2(0, 36);
            var hlg = btnRow.AddComponent<HorizontalLayoutGroup>(); hlg.spacing = 12; hlg.childControlHeight = true; hlg.childForceExpandHeight = true;
            var play = MakeButton(btnRow.transform, "Play");
            var cont = MakeButton(btnRow.transform, "Continue");
            var quit = MakeButton(btnRow.transform, "Quit");

            // Right column (options)
            var right = new GameObject("Right"); right.transform.SetParent(bg.transform, false);
            var rrt = right.AddComponent<RectTransform>(); rrt.anchorMin = new Vector2(0.5f, 0f); rrt.anchorMax = new Vector2(1f, 1f); rrt.offsetMin = new Vector2(12, 24); rrt.offsetMax = new Vector2(-24, -24);
            var rlg = right.AddComponent<VerticalLayoutGroup>(); rlg.spacing = 6; rlg.childControlHeight = true; rlg.childForceExpandHeight = false;

            // Quality dropdown
            MakeHeaderLoc(right.transform, "quality_header");
            var qRow = MakeRowLoc(right.transform, "preset_label"); var qdd = qRow.AddComponent<Dropdown>(); qRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);
            qRow.AddComponent<MMORPG.Client.UI.QualityDropdown>();
            // Language selection
            MakeHeaderLoc(right.transform, "language_header");
            var langRow = MakeRowLoc(right.transform, "language_label"); var langDd = langRow.AddComponent<Dropdown>();
            langRow.AddComponent<MMORPG.Client.UI.LanguageDropdown>();

            // Display options (resolution/fullscreen/vsync)
            MakeHeaderLoc(right.transform, "display_header");
            var resRow = MakeRowLoc(right.transform, "resolution_label"); var resDd = resRow.AddComponent<Dropdown>();
            var fsRow = MakeRowLoc(right.transform, "fullscreen_label"); var fsToggle = fsRow.AddComponent<Toggle>(); fsRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);
            var vsRow = MakeRowLoc(right.transform, "vsync_label"); var vsToggle = vsRow.AddComponent<Toggle>(); vsRow.AddComponent<Image>().color = new Color(0,0,0,0.2f);
            var display = right.AddComponent<MMORPG.Client.UI.DisplayOptions>();
            Assign(display, "resolutionDropdown", resDd);
            Assign(display, "fullscreenToggle", fsToggle);
            Assign(display, "vsyncToggle", vsToggle);

            // FPS Settings (sens/FOV)
            MakeHeaderLoc(right.transform, "first_person_header");
            var sensX = MakeSliderLoc(right.transform, "sensitivity_x", 0.5f, 10f, 2.4f);
            var sensY = MakeSliderLoc(right.transform, "sensitivity_y", 0.5f, 10f, 2.0f);
            var fov = MakeSliderLoc(right.transform, "fov", 50f, 110f, 70f);
            var inv = MakeToggleLoc(right.transform, "invert_y", false);
            var fpsui = right.AddComponent<MMORPG.Client.UI.FPSSettingsUI>();
            Assign(fpsui, "sensitivityX", sensX);
            Assign(fpsui, "sensitivityY", sensY);
            Assign(fpsui, "invertY", inv);
            Assign(fpsui, "fov", fov);

            // Loading overlay
            var overlay = new GameObject("LoadingOverlay"); overlay.transform.SetParent(bg.transform, false);
            var og = overlay.AddComponent<CanvasGroup>(); og.alpha = 0f; og.interactable = false; og.blocksRaycasts = false;
            var ort = overlay.AddComponent<RectTransform>(); ort.anchorMin = Vector2.zero; ort.anchorMax = Vector2.one; ort.offsetMin = Vector2.zero; ort.offsetMax = Vector2.zero;
            var obg = overlay.AddComponent<Image>(); obg.color = new Color(0f, 0f, 0f, 0.6f);
            var prog = new GameObject("Progress"); prog.transform.SetParent(overlay.transform, false);
            var prt = prog.AddComponent<RectTransform>(); prt.anchorMin = new Vector2(0.25f, 0.1f); prt.anchorMax = new Vector2(0.75f, 0.16f);
            var pbg = prog.AddComponent<Image>(); pbg.color = new Color(1,1,1,0.1f);
            var fill = new GameObject("Fill"); fill.transform.SetParent(prog.transform, false);
            var frt = fill.AddComponent<RectTransform>(); frt.anchorMin = new Vector2(0,0); frt.anchorMax = new Vector2(1,1); frt.offsetMin = new Vector2(3,3); frt.offsetMax = new Vector2(-3,-3);
            var pslider = prog.AddComponent<Slider>(); pslider.fillRect = frt; pslider.minValue = 0f; pslider.maxValue = 1f; pslider.value = 0f;
            var ptextGo = new GameObject("Text"); ptextGo.transform.SetParent(overlay.transform, false);
            var ptxt = ptextGo.AddComponent<Text>(); ptxt.text = "Loading..."; ptxt.alignment = TextAnchor.MiddleCenter; ptxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); ptxt.color = Color.white;
            var ptt = ptextGo.GetComponent<RectTransform>(); ptt.anchorMin = new Vector2(0.25f, 0.18f); ptt.anchorMax = new Vector2(0.75f, 0.24f);

            // Launcher controller
            var ctrl = bg.AddComponent<MMORPG.Client.Launcher.LauncherController>();
            Assign(ctrl, "titleText", tText);
            Assign(ctrl, "scenesDropdown", sceneDd);
            Assign(ctrl, "playButton", play);
            Assign(ctrl, "continueButton", cont);
            Assign(ctrl, "profileDropdown", profDd);
            Assign(ctrl, "newProfileInput", newInput);
            Assign(ctrl, "createProfileButton", createBtn);
            Assign(ctrl, "loadingOverlay", og);
            Assign(ctrl, "progressBar", pslider);
            Assign(ctrl, "progressText", ptxt);
            Assign(ctrl, "quitButton", quit);

            // Splash overlay (fades out on start)
            var splash = new GameObject("SplashOverlay"); splash.transform.SetParent(bg.transform, false);
            var sg = splash.AddComponent<CanvasGroup>(); sg.alpha = 1f; sg.blocksRaycasts = true; sg.interactable = true;
            var srt = splash.AddComponent<RectTransform>(); srt.anchorMin = Vector2.zero; srt.anchorMax = Vector2.one; srt.offsetMin = Vector2.zero; srt.offsetMax = Vector2.zero;
            var simg = splash.AddComponent<Image>(); simg.color = Color.black;
            Assign(ctrl, "splashOverlay", sg);

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Launcher.unity");
            EditorUtility.DisplayDialog("Launcher Created", "Generated Launcher scene in Assets/Scenes. Add it as the first scene in Build Settings.", "OK");
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

        static void MakeHeader(Transform parent, string txt)
        {
            var go = new GameObject("Header"); go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>(); t.text = txt; t.fontSize = 16; t.color = Color.white; t.alignment = TextAnchor.MiddleCenter; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 24);
        }
        static void MakeHeaderLoc(Transform parent, string key)
        {
            var go = new GameObject("Header"); go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>(); t.text = key; t.fontSize = 16; t.color = Color.white; t.alignment = TextAnchor.MiddleCenter; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            go.AddComponent<MMORPG.Client.Localization.Localizer>().SetKey(key);
            var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 24);
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
        static GameObject MakeRowLoc(Transform parent, string key)
        {
            var row = new GameObject(key + "Row"); row.transform.SetParent(parent, false);
            var rt = row.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 28);
            var bg = row.AddComponent<Image>(); bg.color = new Color(0,0,0,0.25f);
            var textGo = new GameObject("Label"); textGo.transform.SetParent(row.transform, false);
            var trt = textGo.AddComponent<RectTransform>(); trt.anchorMin = new Vector2(0,0); trt.anchorMax = new Vector2(0.5f,1); trt.offsetMin = new Vector2(6,2); trt.offsetMax = new Vector2(-6,-2);
            var t = textGo.AddComponent<Text>(); t.text = key; t.color = Color.white; t.alignment = TextAnchor.MiddleLeft; t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textGo.AddComponent<MMORPG.Client.Localization.Localizer>().SetKey(key);
            var ctl = new GameObject("Control"); ctl.transform.SetParent(row.transform, false);
            var crt = ctl.AddComponent<RectTransform>(); crt.anchorMin = new Vector2(0.5f,0); crt.anchorMax = new Vector2(1,1); crt.offsetMin = new Vector2(6,2); crt.offsetMax = new Vector2(-6,-2);
            return ctl;
        }

        static Button MakeButton(Transform parent, string label)
        {
            var go = new GameObject(label + "Button"); go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(140, 36);
            var img = go.AddComponent<Image>(); img.color = new Color(0.2f,0.2f,0.25f,0.8f);
            var btn = go.AddComponent<Button>();
            var txt = new GameObject("Text").AddComponent<Text>(); txt.transform.SetParent(go.transform, false); txt.text = label; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;
            txt.gameObject.AddComponent<MMORPG.Client.Localization.Localizer>().SetKey(label.ToLower());
            var trt = txt.GetComponent<RectTransform>(); trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
            return btn;
        }

        static Slider MakeSlider(Transform parent, string label, float min, float max, float value)
        {
            var ctl = MakeRow(parent, label);
            var slider = ctl.AddComponent<Slider>();
            slider.minValue = min; slider.maxValue = max; slider.value = value;
            return slider;
        }
        static Slider MakeSliderLoc(Transform parent, string key, float min, float max, float value)
        {
            var ctl = MakeRowLoc(parent, key);
            var slider = ctl.AddComponent<Slider>(); slider.minValue = min; slider.maxValue = max; slider.value = value; return slider;
        }

        static Toggle MakeToggle(Transform parent, string label, bool value)
        {
            var ctl = MakeRow(parent, label);
            var toggle = ctl.AddComponent<Toggle>(); toggle.isOn = value;
            return toggle;
        }
        static Toggle MakeToggleLoc(Transform parent, string key, bool value)
        {
            var ctl = MakeRowLoc(parent, key);
            var toggle = ctl.AddComponent<Toggle>(); toggle.isOn = value; return toggle;
        }

        static void Assign(object comp, string fieldName, object value)
        {
            var f = comp.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (f != null) f.SetValue(comp, value);
        }
    }
}
