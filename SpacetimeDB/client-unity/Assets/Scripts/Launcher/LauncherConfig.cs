using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Client.Launcher
{
    [CreateAssetMenu(menuName = "MMORPG/Launcher/Config", fileName = "LauncherConfig")]
    public class LauncherConfig : ScriptableObject
    {
        [Tooltip("List of scene names included in Build Settings." )]
        public List<string> sceneNames = new();
        [Tooltip("Default scene index to pre-select in dropdown.")]
        public int defaultSceneIndex = 0;
        [Tooltip("Remember last selected scene via PlayerPrefs.")]
        public bool rememberLastSelection = true;
        [Tooltip("PlayerPrefs key for last selection.")]
        public string prefsKey = "launcher.lastSceneIndex";
        [Header("Branding")]
        public string gameTitle = "MMORPG Demo";
        public Sprite logo;
    }
}

