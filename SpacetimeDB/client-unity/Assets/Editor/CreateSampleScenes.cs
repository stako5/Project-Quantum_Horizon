using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CreateSampleScenes
{
    [MenuItem("Tools/MMORPG/Create Sample Scenes")] 
    public static void CreateScenes()
    {
        CreateCodexScene();
        CreateAvatarScene();
        CreateBootstrapScene();
        CreateSoulsHUDScene();
        CreateBestiaryScene();
        CreateOverworldScene();
        CreateDungeonScene();
        CreateArmorCodexScene();
        CreateContractBoardScene();
        CreateNpcCodexScene();
        CreateConsumablesScene();
        CreateActiveEventsScene();
        CreateSettingsScene();
        CreateQuestLogScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Scenes Created", "Generated Bootstrap, CodexScene, and AvatarScene in Assets/Scenes.", "OK");
    }

    static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
    }

    static GameObject CreateCanvas(out Canvas canvas)
    {
        var go = new GameObject("Canvas");
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static void CreateCodexScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        var panel = new GameObject("CodexPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>(); rt.anchorMin = new Vector2(0,0); rt.anchorMax = new Vector2(1,1); rt.offsetMin = new Vector2(20,20); rt.offsetMax = new Vector2(-20,-20);
        var layout = panel.AddComponent<VerticalLayoutGroup>(); layout.spacing = 8; layout.childForceExpandHeight = false; layout.childControlHeight = true;

        // Dropdown
        var ddGo = new GameObject("ClassDropdown"); ddGo.transform.SetParent(panel.transform, false);
        var dd = ddGo.AddComponent<Dropdown>(); dd.options.Add(new Dropdown.OptionData("Loading..."));
        var label = new GameObject("Label").AddComponent<Text>(); label.transform.SetParent(ddGo.transform, false); label.text = "Select Class"; label.color = Color.white; label.alignment = TextAnchor.MiddleLeft;

        // Abilities scroll
        RectTransform abilitiesContent;
        var abilities = CreateScroll(panel.transform, "Abilities", out abilitiesContent);
        // Perks scroll
        RectTransform perksContent;
        var perks = CreateScroll(panel.transform, "Perks", out perksContent);
        // Item template
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>();
        itemTemplate.transform.SetParent(panel.transform, false);
        itemTemplate.text = "Sample"; itemTemplate.gameObject.SetActive(false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14;

        // CodexUI
        var codex = panel.AddComponent<MMORPG.Client.UI.CodexUI>();
        codex.GetType().GetField("classDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, dd);
        codex.GetType().GetField("abilitiesContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, abilitiesContent);
        codex.GetType().GetField("perksContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, perksContent);
        codex.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, itemTemplate);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/CodexScene.unity");
    }

    static GameObject CreateScroll(Transform parent, string title, out RectTransform content)
    {
        var root = new GameObject(title);
        root.transform.SetParent(parent, false);
        var rt = root.AddComponent<RectTransform>(); rt.sizeDelta = new Vector2(0, 200);
        var image = root.AddComponent<Image>(); image.color = new Color(0,0,0,0.3f);

        var viewport = new GameObject("Viewport"); viewport.transform.SetParent(root.transform, false);
        var vpRT = viewport.AddComponent<RectTransform>(); vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one; vpRT.offsetMin = Vector2.zero; vpRT.offsetMax = Vector2.zero;
        var mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; viewport.AddComponent<Image>();

        var contentGO = new GameObject("Content"); contentGO.transform.SetParent(viewport.transform, false);
        content = contentGO.AddComponent<RectTransform>(); content.anchorMin = new Vector2(0,1); content.anchorMax = new Vector2(1,1); content.pivot = new Vector2(0.5f,1);
        var vlg = contentGO.AddComponent<VerticalLayoutGroup>(); vlg.childForceExpandHeight = false; vlg.childControlHeight = true; vlg.spacing = 4;
        contentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = root.AddComponent<ScrollRect>(); scroll.viewport = vpRT; scroll.content = content;
        return root;
    }

    static Button CreateButton(Transform parent, string text, Vector2 size)
    {
        var go = new GameObject("Button"); go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>(); img.color = new Color(1,1,1,0.12f);
        var btn = go.AddComponent<Button>();
        var label = new GameObject("Text").AddComponent<Text>(); label.transform.SetParent(go.transform, false); label.text = text; label.color = Color.white; label.alignment = TextAnchor.MiddleCenter;
        var rt = go.GetComponent<RectTransform>(); rt.sizeDelta = size;
        return btn;
    }

    static void CreateAvatarScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        var panel = new GameObject("AvatarPanel"); panel.transform.SetParent(canvasGo.transform, false);
        panel.AddComponent<VerticalLayoutGroup>();

        // Create sliders helper
        Slider MakeSlider(string name, float min, float max, float value)
        {
            var go = new GameObject(name); go.transform.SetParent(panel.transform, false);
            var slider = go.AddComponent<Slider>(); slider.minValue = min; slider.maxValue = max; slider.value = value; return slider;
        }

        var height = MakeSlider("Height", 145, 210, 175);
        var weight = MakeSlider("Weight", 35, 180, 70);
        var torso  = MakeSlider("TorsoScale", 0.9f, 1.1f, 1.0f);
        var arm    = MakeSlider("ArmLength", 0.85f, 1.15f, 1.0f);
        var leg    = MakeSlider("LegLength", 0.85f, 1.15f, 1.0f);

        var avatarGO = new GameObject("Avatar");
        var applier = avatarGO.AddComponent<MMORPG.Client.Avatar.AvatarApplier>();
        var ui = panel.AddComponent<MMORPG.Client.Avatar.CustomizationUI>();
        ui.GetType().GetField("applier", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, applier);
        ui.GetType().GetField("heightSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, height);
        ui.GetType().GetField("weightSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, weight);
        ui.GetType().GetField("torsoSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, torso);
        ui.GetType().GetField("armSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, arm);
        ui.GetType().GetField("legSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, leg);

        var uploader = panel.AddComponent<MMORPG.Client.Avatar.AvatarUploader>();
        uploader.GetType().GetField("customization", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(uploader, ui);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/AvatarScene.unity");
    }

    static void CreateBootstrapScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var go = new GameObject("SpacetimeBootstrap");
        go.AddComponent<MMORPG.Client.Net.SpacetimeDbClient>();
        go.AddComponent<MMORPG.Client.Net.BindingsProviders>();
        go.AddComponent<MMORPG.Client.Net.SpacetimeDbSubscriptions>();
        go.AddComponent<MMORPG.Client.Quests.QuestsBootstrapper>();

        // HUD Canvas
        var canvasGo = CreateCanvas(out var canvas);
        var textGo = new GameObject("InstanceText"); textGo.transform.SetParent(canvasGo.transform, false);
        var text = textGo.AddComponent<Text>(); text.color = Color.white; text.alignment = TextAnchor.UpperLeft; text.fontSize = 16;
        var cgrt = text.GetComponent<RectTransform>(); cgrt.anchorMin = new Vector2(0,1); cgrt.anchorMax = new Vector2(0,1); cgrt.pivot = new Vector2(0,1); cgrt.anchoredPosition = new Vector2(20,-20); cgrt.sizeDelta = new Vector2(600,80);
        var hud = canvasGo.AddComponent<MMORPG.Client.UI.InstanceHUD>();
        hud.GetType().GetField("statusText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, text);

        // Instance Manager
        var mgr = new GameObject("DungeonInstanceManager").AddComponent<MMORPG.Client.Dungeon.DungeonInstanceManager>();
        mgr.GetType().GetField("hud", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(mgr, hud);

        // Remote players manager
        new GameObject("RemotePlayers").AddComponent<MMORPG.Client.Net.RemotePlayersManager>();

        // Region dropdown (top-left)
        var ddGo = new GameObject("RegionDropdown"); ddGo.transform.SetParent(canvasGo.transform, false);
        var ddrt = ddGo.AddComponent<RectTransform>(); ddrt.anchorMin = new Vector2(0,1); ddrt.anchorMax = new Vector2(0,1); ddrt.pivot = new Vector2(0,1); ddrt.anchoredPosition = new Vector2(24,-64); ddrt.sizeDelta = new Vector2(200,32);
        var dd = ddGo.AddComponent<Dropdown>(); dd.options.Add(new Dropdown.OptionData("Region 1"));
        var ddLabel = new GameObject("Label").AddComponent<Text>(); ddLabel.transform.SetParent(ddGo.transform, false); ddLabel.text = "Region"; ddLabel.color = Color.white; ddLabel.alignment = TextAnchor.MiddleLeft;
        ddGo.AddComponent<MMORPG.Client.UI.RegionDropdownUI>();

        // NPCs bootstrapper
        canvasGo.AddComponent<MMORPG.Client.NPC.NpcsBootstrapper>();
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Bootstrap.unity");
    }

    static void CreateSoulsHUDScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);

        // HP/FP/STA bars (top-left)
        GameObject MakeBar(string name, Color c)
        {
            var go = new GameObject(name); go.transform.SetParent(canvasGo.transform, false);
            var rt = go.AddComponent<RectTransform>(); rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = Vector2.zero; rt.sizeDelta = new Vector2(300, 16);
            var bg = go.AddComponent<Image>(); bg.color = new Color(0,0,0,0.5f);
            var fillGo = new GameObject("Fill"); fillGo.transform.SetParent(go.transform, false);
            var frt = fillGo.AddComponent<RectTransform>(); frt.anchorMin = new Vector2(0,0); frt.anchorMax = new Vector2(1,1); frt.offsetMin = new Vector2(2,2); frt.offsetMax = new Vector2(-2,-2);
            var fill = fillGo.AddComponent<Image>(); fill.color = c; fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillAmount = 1f;
            return go;
        }

        var hp = MakeBar("HP", new Color(0.8f,0.1f,0.1f));
        var fp = MakeBar("FP", new Color(0.2f,0.4f,0.9f));
        var sta = MakeBar("Stamina", new Color(0.2f,0.8f,0.2f));
        hp.GetComponent<RectTransform>().anchoredPosition = new Vector2(24, -24);
        fp.GetComponent<RectTransform>().anchoredPosition = new Vector2(24, -48);
        sta.GetComponent<RectTransform>().anchoredPosition = new Vector2(24, -72);

        // Quick slots (bottom-left)
        var slotsRoot = new GameObject("QuickSlots"); slotsRoot.transform.SetParent(canvasGo.transform, false);
        var srt = slotsRoot.AddComponent<RectTransform>(); srt.anchorMin = new Vector2(0,0); srt.anchorMax = new Vector2(0,0); srt.pivot = new Vector2(0,0); srt.anchoredPosition = new Vector2(24,24);
        var hlg = slotsRoot.AddComponent<HorizontalLayoutGroup>(); hlg.spacing = 8;
        Image[] slotImgs = new Image[4];
        for (int i=0;i<4;i++) { var s = new GameObject($"Slot{i+1}"); s.transform.SetParent(slotsRoot.transform, false); var img = s.AddComponent<Image>(); img.color = new Color(1,1,1,0.15f); img.rectTransform.sizeDelta = new Vector2(64,64); slotImgs[i] = img; }

        // Souls counter (bottom-right)
        var soulsGo = new GameObject("Souls"); soulsGo.transform.SetParent(canvasGo.transform, false);
        var srt2 = soulsGo.AddComponent<RectTransform>(); srt2.anchorMin = new Vector2(1,0); srt2.anchorMax = new Vector2(1,0); srt2.pivot = new Vector2(1,0); srt2.anchoredPosition = new Vector2(-24,24);
        var soulsText = soulsGo.AddComponent<Text>(); soulsText.color = Color.white; soulsText.fontSize = 24; soulsText.alignment = TextAnchor.MiddleRight; soulsText.text = "0";

        // Boss bar (bottom-center)
        var bossRoot = new GameObject("BossBar"); bossRoot.transform.SetParent(canvasGo.transform, false);
        var brt = bossRoot.AddComponent<RectTransform>(); brt.anchorMin = new Vector2(0.5f,0); brt.anchorMax = new Vector2(0.5f,0); brt.pivot = new Vector2(0.5f,0); brt.anchoredPosition = new Vector2(0,80); brt.sizeDelta = new Vector2(600, 24);
        var bbBg = bossRoot.AddComponent<Image>(); bbBg.color = new Color(0,0,0,0.6f);
        var fillGo2 = new GameObject("Fill"); fillGo2.transform.SetParent(bossRoot.transform, false);
        var f2rt = fillGo2.AddComponent<RectTransform>(); f2rt.anchorMin = new Vector2(0,0); f2rt.anchorMax = new Vector2(1,1); f2rt.offsetMin = new Vector2(2,2); f2rt.offsetMax = new Vector2(-2,-2);
        var bbFill = fillGo2.AddComponent<Image>(); bbFill.color = new Color(0.8f,0.1f,0.1f); bbFill.type = Image.Type.Filled; bbFill.fillMethod = Image.FillMethod.Horizontal; bbFill.fillAmount = 1f;
        var bossName = new GameObject("Name").AddComponent<Text>(); bossName.transform.SetParent(bossRoot.transform, false); bossName.alignment = TextAnchor.MiddleCenter; bossName.text = "BOSS"; bossName.color = Color.white;
        var cg = bossRoot.AddComponent<CanvasGroup>(); cg.alpha = 0f;

        // Attach controllers
        var hud = canvasGo.AddComponent<MMORPG.Client.UI.SoulsHUD>();
        hud.GetType().GetField("hpFill", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, hp.transform.Find("Fill").GetComponent<Image>());
        hud.GetType().GetField("fpFill", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, fp.transform.Find("Fill").GetComponent<Image>());
        hud.GetType().GetField("staFill", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, sta.transform.Find("Fill").GetComponent<Image>());
        hud.GetType().GetField("quickSlots", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, slotImgs);
        hud.GetType().GetField("soulsText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, soulsText);

        var bb = canvasGo.AddComponent<MMORPG.Client.UI.BossBarUI>();
        bb.GetType().GetField("bossNameText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(bb, bossName);
        bb.GetType().GetField("healthFill", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(bb, bbFill);
        bb.GetType().GetField("canvasGroup", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(bb, cg);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/SoulsHUDScene.unity");
    }

    static void CreateBestiaryScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);

        var panel = new GameObject("BestiaryPanel"); panel.transform.SetParent(canvasGo.transform, false);
        var vlg = panel.AddComponent<VerticalLayoutGroup>(); vlg.spacing = 6; vlg.childForceExpandHeight = false; vlg.childControlHeight = true;

        Dropdown MakeDropdown(string name)
        {
            var go = new GameObject(name); go.transform.SetParent(panel.transform, false); return go.AddComponent<Dropdown>();
        }
        var family = MakeDropdown("Family"); var role = MakeDropdown("Role"); var elem = MakeDropdown("Element"); var tier = MakeDropdown("Tier");

        RectTransform content;
        CreateScroll(panel.transform, "List", out content);
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(panel.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);

        // Detail area
        var detailPanel = new GameObject("Detail"); detailPanel.transform.SetParent(panel.transform, false);
        var dVlg = detailPanel.AddComponent<VerticalLayoutGroup>(); dVlg.spacing = 4; dVlg.childForceExpandHeight = false; dVlg.childControlHeight = true;
        var nameT = new GameObject("Name").AddComponent<Text>(); nameT.transform.SetParent(detailPanel.transform, false); nameT.color = Color.white; nameT.fontSize = 18; nameT.alignment = TextAnchor.MiddleLeft;
        var statsT = new GameObject("Stats").AddComponent<Text>(); statsT.transform.SetParent(detailPanel.transform, false); statsT.color = Color.white; statsT.fontSize = 14; statsT.alignment = TextAnchor.UpperLeft;
        var descT = new GameObject("Desc").AddComponent<Text>(); descT.transform.SetParent(detailPanel.transform, false); descT.color = Color.white; descT.fontSize = 12; descT.alignment = TextAnchor.UpperLeft;
        var previewHolder = new GameObject("Preview"); previewHolder.transform.SetParent(detailPanel.transform, false);
        var previewRT = previewHolder.AddComponent<RawImage>(); previewRT.color = Color.black; previewRT.rectTransform.sizeDelta = new Vector2(256,256);
        var preview = detailPanel.AddComponent<MMORPG.Client.UI.BestiaryPreviewRenderer>();

        var detail = detailPanel.AddComponent<MMORPG.Client.UI.BestiaryDetailUI>();
        detail.GetType().GetField("nameText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(detail, nameT);
        detail.GetType().GetField("statsText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(detail, statsT);
        detail.GetType().GetField("descText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(detail, descT);
        detail.GetType().GetField("previewRenderer", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(detail, preview);
        detail.GetType().GetField("previewImage", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(detail, previewRT);

        var ui = panel.AddComponent<MMORPG.Client.UI.BestiaryUI>();
        ui.GetType().GetField("familyDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, family);
        ui.GetType().GetField("roleDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, role);
        ui.GetType().GetField("elementDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, elem);
        ui.GetType().GetField("tierDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, tier);
        ui.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, content);
        ui.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, itemTemplate);
        ui.GetType().GetField("detailUI", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, detail);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/BestiaryScene.unity");
    }

    static void CreateOverworldScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        var grid = new GameObject("OverworldGrid"); grid.transform.SetParent(canvasGo.transform, false);
        var gLayout = grid.AddComponent<GridLayoutGroup>(); gLayout.cellSize = new Vector2(160,80); gLayout.spacing = new Vector2(8,8);
        // Create 10 buttons for 10 biomes
        for (int i=0;i<10;i++)
        {
            var btnGO = new GameObject($"Biome{i+1}"); btnGO.transform.SetParent(grid.transform, false);
            var img = btnGO.AddComponent<Image>(); img.color = new Color(1,1,1,0.1f);
            var btn = btnGO.AddComponent<Button>();
            var label = new GameObject("Text").AddComponent<Text>(); label.transform.SetParent(btnGO.transform, false); label.alignment = TextAnchor.MiddleCenter; label.color = Color.white; label.text = $"Biome {i+1}";
        }
        var managerGO = new GameObject("WorldRegionManager");
        var manager = managerGO.AddComponent<MMORPG.Client.World.WorldRegionManager>();
        
        // Player placeholder + camera
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule); player.name = "Player"; player.transform.position = Vector3.zero;
        var netSync = player.AddComponent<MMORPG.Client.Net.PlayerNetSync>();
        var cam = Camera.main ?? new GameObject("Main Camera").AddComponent<Camera>(); cam.tag = "MainCamera"; cam.transform.position = player.transform.position + new Vector3(0,2,-6); cam.transform.LookAt(player.transform);

        // HUD + InstanceManager
        var textGo = new GameObject("InstanceText"); textGo.transform.SetParent(canvasGo.transform, false);
        var text = textGo.AddComponent<Text>(); text.color = Color.white; text.alignment = TextAnchor.UpperLeft; text.fontSize = 16;
        var cgrt = text.GetComponent<RectTransform>(); cgrt.anchorMin = new Vector2(0,1); cgrt.anchorMax = new Vector2(0,1); cgrt.pivot = new Vector2(0,1); cgrt.anchoredPosition = new Vector2(20,-20); cgrt.sizeDelta = new Vector2(600,80);
        var hud = canvasGo.AddComponent<MMORPG.Client.UI.InstanceHUD>();
        hud.GetType().GetField("statusText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(hud, text);
        var mgr = new GameObject("DungeonInstanceManager").AddComponent<MMORPG.Client.Dungeon.DungeonInstanceManager>();
        mgr.GetType().GetField("hud", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(mgr, hud);

        // Remote players manager
        new GameObject("RemotePlayers").AddComponent<MMORPG.Client.Net.RemotePlayersManager>();

        // Region dropdown (top-left)
        var ddGo = new GameObject("RegionDropdown"); ddGo.transform.SetParent(canvasGo.transform, false);
        var ddrt = ddGo.AddComponent<RectTransform>(); ddrt.anchorMin = new Vector2(0,1); ddrt.anchorMax = new Vector2(0,1); ddrt.pivot = new Vector2(0,1); ddrt.anchoredPosition = new Vector2(24,-64); ddrt.sizeDelta = new Vector2(200,32);
        var dd = ddGo.AddComponent<Dropdown>(); dd.options.Add(new Dropdown.OptionData("Region 1"));
        var ddLabel = new GameObject("Label").AddComponent<Text>(); ddLabel.transform.SetParent(ddGo.transform, false); ddLabel.text = "Region"; ddLabel.color = Color.white; ddLabel.alignment = TextAnchor.MiddleLeft;
        ddGo.AddComponent<MMORPG.Client.UI.RegionDropdownUI>();

        // NPCs bootstrapper
        canvasGo.AddComponent<MMORPG.Client.NPC.NpcsBootstrapper>();
        mgr.SetPlayer(player.transform);

        // World Boss Effects + Markers
        var effect = new GameObject("WorldBossEffects").AddComponent<MMORPG.Client.World.WorldBossEffectApplier>();
        effect.GetType().GetField("player", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(effect, player.transform);
        var markerMgr = new GameObject("WorldBossMarkers").AddComponent<MMORPG.Client.World.WorldBossMarkerManager>();
        var markerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Portals/WorldBossMarker.prefab");
        if (markerPrefab)
        {
            markerMgr.GetType().GetField("markerPrefab", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(markerMgr, markerPrefab);
        }
        // Shared Event Markers reuse the same prefab for simplicity
        var eventMarkerMgr = new GameObject("WorldEventMarkers").AddComponent<MMORPG.Client.World.WorldEventMarkerManager>();
        if (markerPrefab)
        {
            eventMarkerMgr.GetType().GetField("markerPrefab", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(eventMarkerMgr, markerPrefab);
        }
        // World Boss Map UI (panel + list)
        RectTransform wbListContent; CreateScroll(canvasGo.transform, "WorldBossMap", out wbListContent);
        var wbItemTemplate = new GameObject("WBItemTemplate").AddComponent<Text>();
        wbItemTemplate.transform.SetParent(canvasGo.transform, false);
        wbItemTemplate.color = Color.white; wbItemTemplate.fontSize = 14; wbItemTemplate.gameObject.SetActive(false);
        var mapUI = canvasGo.AddComponent<MMORPG.Client.UI.WorldBossMapUI>();
        mapUI.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(mapUI, wbListContent);
        mapUI.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(mapUI, wbItemTemplate);

        // Active Events Panel (list)
        RectTransform evListContent; CreateScroll(canvasGo.transform, "ActiveEvents", out evListContent);
        var evItemTemplate = new GameObject("EvItemTemplate").AddComponent<Text>(); evItemTemplate.transform.SetParent(canvasGo.transform, false); evItemTemplate.color = Color.white; evItemTemplate.fontSize = 14; evItemTemplate.gameObject.SetActive(false);
        var evUI = canvasGo.AddComponent<MMORPG.Client.UI.ActiveEventsUI>();
        evUI.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(evUI, evListContent);
        evUI.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(evUI, evItemTemplate);

        // Remote Players manager (shows other users in same region)
        var remoteMgr = new GameObject("RemotePlayers").AddComponent<MMORPG.Client.Net.RemotePlayersManager>();

        // Sample cognitive NPC
        var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule); npc.name = "SampleNPC"; npc.transform.position = new Vector3(6,0,6);
        npc.AddComponent<MMORPG.Client.NPC.Cognition.NpcCognitiveMemory>();
        var cog = npc.AddComponent<MMORPG.Client.NPC.Cognition.NpcCognition>();
        var pers = ScriptableObject.CreateInstance<MMORPG.Client.NPC.Cognition.NpcPersonality>();
        pers.extraversion = 0.7f; pers.openness = 0.8f; pers.conscientiousness = 0.6f; pers.prefersExploration = 0.2f; pers.prefersSocial = 0.3f;
        cog.GetType().GetField("personality", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic).SetValue(cog, pers);
        var agent = npc.AddComponent<MMORPG.Client.AI.UtilityAgent>();
        var wander = ScriptableObject.CreateInstance<MMORPG.Client.AI.Actions.NpcActionWander>(); wander.weight = 1f;
        var work = ScriptableObject.CreateInstance<MMORPG.Client.AI.Actions.NpcActionWork>(); work.weight = 1f;
        var social = ScriptableObject.CreateInstance<MMORPG.Client.AI.Actions.NpcActionSocialize>(); social.weight = 1f;
        var rest = ScriptableObject.CreateInstance<MMORPG.Client.AI.Actions.NpcActionRest>(); rest.weight = 0.8f;
        var sleep = ScriptableObject.CreateInstance<MMORPG.Client.AI.Actions.NpcActionSleep>(); sleep.weight = 1.2f;
        var listField2 = typeof(MMORPG.Client.AI.UtilityAgent).GetField("actions", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
        var list2 = new System.Collections.Generic.List<MMORPG.Client.AI.UtilityAction> { wander, work, social, rest, sleep };
        listField2.SetValue(agent, list2);

        // Sync server personality traits onto sample NPC (use first available NPC if none specified)
        npc.AddComponent<MMORPG.Client.NPC.NpcTraitSync>();

        // Interact (talk) with NPC for quest triggers
        var interact = npc.AddComponent<MMORPG.Client.NPC.NpcInteractable>();
        interact.GetType().GetField("interactRange", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(interact, 3.0f);
        interact.GetType().GetField("player", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(interact, player.transform);

        // NPC Action Debug UI (top-left under status)
        var actLblGo = new GameObject("NpcActionLabel"); actLblGo.transform.SetParent(canvasGo.transform, false);
        var actText = actLblGo.AddComponent<Text>(); actText.color = Color.cyan; actText.alignment = TextAnchor.UpperLeft; actText.fontSize = 14; actText.text = "Action: (init)";
        var alrt = actText.GetComponent<RectTransform>(); alrt.anchorMin = new Vector2(0,1); alrt.anchorMax = new Vector2(0,1); alrt.pivot = new Vector2(0,1); alrt.anchoredPosition = new Vector2(20,-48); alrt.sizeDelta = new Vector2(360,24);
        var actDbg = canvasGo.AddComponent<MMORPG.Client.UI.NpcActionDebugUI>();
        actDbg.GetType().GetField("agent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(actDbg, agent);
        actDbg.GetType().GetField("label", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(actDbg, actText);


        // Region dropdown (top-left)
        var ddGo = new GameObject("RegionDropdown"); ddGo.transform.SetParent(canvasGo.transform, false);
        var ddrt = ddGo.AddComponent<RectTransform>(); ddrt.anchorMin = new Vector2(0,1); ddrt.anchorMax = new Vector2(0,1); ddrt.pivot = new Vector2(0,1); ddrt.anchoredPosition = new Vector2(24,-24); ddrt.sizeDelta = new Vector2(200,32);
        var dd = ddGo.AddComponent<Dropdown>(); dd.options.Add(new Dropdown.OptionData("Region 1"));
        var ddLabel = new GameObject("Label").AddComponent<Text>(); ddLabel.transform.SetParent(ddGo.transform, false); ddLabel.text = "Region"; ddLabel.color = Color.white; ddLabel.alignment = TextAnchor.MiddleLeft;
        var regSel = ddGo.AddComponent<MMORPG.Client.UI.RegionDropdownUI>();

        // NPCs bootstrapper
        canvasGo.AddComponent<MMORPG.Client.NPC.NpcsBootstrapper>();
        regSel.GetType().GetField("dropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(regSel, dd);
        // Hook buttons
        int idx = 0;
        foreach (Transform child in grid.transform)
        {
            int captured = idx;
            child.GetComponent<Button>().onClick.AddListener(() => {
                manager.ApplyRegionByIndex(captured);
                // Update player's networked region and filter remote players by region
                netSync.SetRegion((byte)captured);
                var f = remoteMgr.GetType().GetField("localRegionId", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
                if (f != null) f.SetValue(remoteMgr, (byte)captured);
            });
            idx++;
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Overworld.unity");
    }

    static void CreateDungeonScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var genGO = new GameObject("DungeonGenerator");
        var gen = genGO.AddComponent<MMORPG.Client.Dungeon.DungeonGenerator>();
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/DungeonScene.unity");
    }

    static void CreateArmorCodexScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        // Dropdown for category
        var ddGo = new GameObject("Category"); ddGo.transform.SetParent(canvasGo.transform, false);
        var dd = ddGo.AddComponent<Dropdown>(); dd.AddOptions(new System.Collections.Generic.List<string>{"All","World","Faction","Enemy","Boss"});
        // Scroll for sets
        RectTransform setsContent; CreateScroll(canvasGo.transform, "Sets", out setsContent);
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(canvasGo.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);
        // ArmorCodexUI
        var codex = canvasGo.AddComponent<MMORPG.Client.UI.ArmorCodexUI>();
        codex.GetType().GetField("categoryDropdown", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, dd);
        codex.GetType().GetField("setsContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, setsContent);
        codex.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, itemTemplate);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/ArmorCodexScene.unity");
    }

    static void CreateContractBoardScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        RectTransform content; CreateScroll(canvasGo.transform, "Contracts", out content);
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(canvasGo.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);
        var refresh = CreateButton(canvasGo.transform, "Refresh", new Vector2(120, 32));
        var board = canvasGo.AddComponent<MMORPG.Client.UI.FactionContractBoard>();
        board.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(board, content);
        board.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(board, itemTemplate);
        board.GetType().GetField("refreshButton", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(board, refresh);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/ContractBoardScene.unity");
    }

    static void CreateConsumablesScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);

        // Player + Buff Manager
        var player = new GameObject("Player");
        var buffMgr = player.AddComponent<MMORPG.Client.Combat.BuffManager>();
        player.AddComponent<MMORPG.Client.UI.BuffSyncFromServer>();
        var consumer = player.AddComponent<MMORPG.Client.Items.ConsumableUse>();
        player.AddComponent<MMORPG.Client.Items.ConsumablesBootstrapper>();

        // Scroll list
        RectTransform content; CreateScroll(canvasGo.transform, "Consumables", out content);
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(canvasGo.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);

        // Wire list UI
        var list = canvasGo.AddComponent<MMORPG.Client.UI.ConsumablesUI>();
        list.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(list, content);
        list.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(list, itemTemplate);
        list.GetType().GetField("consumer", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(list, consumer);

        // Debug modifiers panel
        var dbgGo = new GameObject("BuffsDebug"); dbgGo.transform.SetParent(canvasGo.transform, false);
        var dbgTxt = dbgGo.AddComponent<Text>(); dbgTxt.color = Color.white; dbgTxt.alignment = TextAnchor.UpperLeft; dbgTxt.fontSize = 14;
        var drt = dbgGo.GetComponent<RectTransform>(); drt.anchorMin = new Vector2(1,1); drt.anchorMax = new Vector2(1,1); drt.pivot = new Vector2(1,1); drt.anchoredPosition = new Vector2(-20,-20); drt.sizeDelta = new Vector2(520, 320);
        var dbg = canvasGo.AddComponent<MMORPG.Client.UI.ActiveBuffsDebugUI>();
        dbg.GetType().GetField("manager", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(dbg, buffMgr);
        dbg.GetType().GetField("text", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(dbg, dbgTxt);

        // Buff Bar (top-left)
        RectTransform buffContent; CreateScroll(canvasGo.transform, "BuffBar", out buffContent);
        var buffItemTemplate = new GameObject("BuffItemTemplate").AddComponent<Text>(); buffItemTemplate.transform.SetParent(canvasGo.transform, false); buffItemTemplate.color = Color.white; buffItemTemplate.fontSize = 12; buffItemTemplate.gameObject.SetActive(false);
        var buffBar = canvasGo.AddComponent<MMORPG.Client.UI.BuffBarUI>();
        buffBar.GetType().GetField("buffManager", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(buffBar, buffMgr);
        buffBar.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(buffBar, buffContent);
        buffBar.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(buffBar, buffItemTemplate);

        // Quick Slots (bottom-left)
        var qsRoot = new GameObject("QuickSlots"); qsRoot.transform.SetParent(canvasGo.transform, false);
        var qsrt = qsRoot.AddComponent<RectTransform>(); qsrt.anchorMin = new Vector2(0,0); qsrt.anchorMax = new Vector2(0,0); qsrt.pivot = new Vector2(0,0); qsrt.anchoredPosition = new Vector2(24,24);
        var qs = qsRoot.AddComponent<MMORPG.Client.UI.QuickSlotsUI>();
        qs.GetType().GetField("consumer", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(qs, consumer);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/ConsumablesScene.unity");
    }

    static void CreateActiveEventsScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        RectTransform evListContent; CreateScroll(canvasGo.transform, "ActiveEvents", out evListContent);
        var evItemTemplate = new GameObject("EvItemTemplate").AddComponent<Text>(); evItemTemplate.transform.SetParent(canvasGo.transform, false); evItemTemplate.color = Color.white; evItemTemplate.fontSize = 14; evItemTemplate.gameObject.SetActive(false);
        var evUI = canvasGo.AddComponent<MMORPG.Client.UI.ActiveEventsUI>();
        evUI.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(evUI, evListContent);
        evUI.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(evUI, evItemTemplate);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/ActiveEventsScene.unity");
    }


    static void CreateSettingsScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);

        // Ensure InputManager
        var im = new GameObject("InputManager").AddComponent<MMORPG.Client.InputSystem.InputManager>();

        // Settings panel
        RectTransform content; CreateScroll(canvasGo.transform, "GameSettings", out content);
        var itemTemplate = new GameObject("ItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(canvasGo.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);
        var settings = canvasGo.AddComponent<MMORPG.Client.UI.GameSettingsUI>();
        settings.GetType().GetField("content", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(settings, content);
        settings.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(settings, itemTemplate);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/SettingsScene.unity");
    }


    static void CreateNpcCodexScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);

        // Search bar
        var searchGo = new GameObject("Search"); searchGo.transform.SetParent(canvasGo.transform, false);
        var search = searchGo.AddComponent<InputField>(); var searchText = new GameObject("Text").AddComponent<Text>(); searchText.transform.SetParent(searchGo.transform, false); searchText.color = Color.white; searchText.alignment = TextAnchor.MiddleLeft;
        var srt = searchGo.GetComponent<RectTransform>(); srt.anchorMin = new Vector2(0,1); srt.anchorMax = new Vector2(0,1); srt.pivot = new Vector2(0,1); srt.anchoredPosition = new Vector2(20,-20); srt.sizeDelta = new Vector2(300,30);

        // List (left)
        RectTransform listContent; CreateScroll(canvasGo.transform, "NpcList", out listContent);
        listContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(20,-60);
        var listItemTemplate = new GameObject("ListItemTemplate").AddComponent<Text>(); listItemTemplate.transform.SetParent(canvasGo.transform, false); listItemTemplate.color = Color.white; listItemTemplate.fontSize = 14; listItemTemplate.gameObject.SetActive(false);

        // Detail (right)
        RectTransform detailContent; CreateScroll(canvasGo.transform, "NpcDetail", out detailContent);
        detailContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(400,-60);
        var detailLineTemplate = new GameObject("DetailLineTemplate").AddComponent<Text>(); detailLineTemplate.transform.SetParent(canvasGo.transform, false); detailLineTemplate.color = Color.white; detailLineTemplate.fontSize = 14; detailLineTemplate.gameObject.SetActive(false);

        // Memories (bottom-right)
        RectTransform memContent; CreateScroll(canvasGo.transform, "NpcMemories", out memContent);
        memContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(400,-300);
        var memItemTemplate = new GameObject("MemItemTemplate").AddComponent<Text>(); memItemTemplate.transform.SetParent(canvasGo.transform, false); memItemTemplate.color = Color.white; memItemTemplate.fontSize = 12; memItemTemplate.gameObject.SetActive(false);

        // Offered quests (bottom-left)
        RectTransform offeredContent; CreateScroll(canvasGo.transform, "NpcOfferedQuests", out offeredContent);
        offeredContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(20,-300);
        var offeredItemTemplate = new GameObject("OfferedItemTemplate").AddComponent<Text>(); offeredItemTemplate.transform.SetParent(canvasGo.transform, false); offeredItemTemplate.color = Color.white; offeredItemTemplate.fontSize = 12; offeredItemTemplate.gameObject.SetActive(false);

        // Personality panel (right, above memories)
        var persPanel = new GameObject("Personality"); persPanel.transform.SetParent(canvasGo.transform, false);
        var prt = persPanel.AddComponent<RectTransform>(); prt.anchorMin = new Vector2(0,1); prt.anchorMax = new Vector2(0,1); prt.pivot = new Vector2(0,1); prt.anchoredPosition = new Vector2(400,-60); prt.sizeDelta = new Vector2(320, 120);
        var pvlg = persPanel.AddComponent<VerticalLayoutGroup>(); pvlg.spacing = 4;
        Slider MakeSlider(string label)
        {
            var go = new GameObject(label); go.transform.SetParent(persPanel.transform, false);
            var s = go.AddComponent<Slider>(); s.minValue = 0f; s.maxValue = 1f; s.interactable = false; return s;
        }
        var oSl = MakeSlider("Openness"); var cSl = MakeSlider("Conscientiousness"); var eSl = MakeSlider("Extraversion");
        var persTextGo = new GameObject("TraitText"); persTextGo.transform.SetParent(persPanel.transform, false); var persText = persTextGo.AddComponent<Text>(); persText.color = Color.white; persText.fontSize = 12; persText.alignment = TextAnchor.MiddleLeft; persText.text = "O: C: E:";

        // Wire UI
        var codex = canvasGo.AddComponent<MMORPG.Client.UI.NpcCodexUI>();
        codex.GetType().GetField("listContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, listContent);
        codex.GetType().GetField("listItemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, listItemTemplate);
        codex.GetType().GetField("detailContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, detailContent);
        codex.GetType().GetField("detailLineTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, detailLineTemplate);
        codex.GetType().GetField("memoriesContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, memContent);
        codex.GetType().GetField("memoryItemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, memItemTemplate);
        codex.GetType().GetField("searchField", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, search);
        codex.GetType().GetField("offeredContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, offeredContent);
        codex.GetType().GetField("offeredItemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, offeredItemTemplate);
        codex.GetType().GetField("opennessSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, oSl);
        codex.GetType().GetField("conscientiousnessSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, cSl);
        codex.GetType().GetField("extraversionSlider", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, eSl);
        codex.GetType().GetField("personalitySummaryText", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(codex, persText);

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/NpcCodexScene.unity");
    }


    static void CreateQuestLogScene()
    {
        EnsureScenesFolder();
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
        var canvasGo = CreateCanvas(out var canvas);
        // Available list
        RectTransform avail; CreateScroll(canvasGo.transform, "AvailableQuests", out avail);
        // Active list
        RectTransform active; CreateScroll(canvasGo.transform, "ActiveQuests", out active);
        var itemTemplate = new GameObject("QuestItemTemplate").AddComponent<Text>(); itemTemplate.transform.SetParent(canvasGo.transform, false); itemTemplate.color = Color.white; itemTemplate.fontSize = 14; itemTemplate.gameObject.SetActive(false);
        var ui = canvasGo.AddComponent<MMORPG.Client.UI.QuestLogUI>();
        ui.GetType().GetField("availableContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, avail);
        ui.GetType().GetField("activeContent", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, active);
        ui.GetType().GetField("itemTemplate", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(ui, itemTemplate);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/QuestLogScene.unity");
    }
}
