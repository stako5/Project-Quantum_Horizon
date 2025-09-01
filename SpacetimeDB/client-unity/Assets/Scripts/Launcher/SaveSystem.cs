using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MMORPG.Client.Launcher
{
    [Serializable]
    public class SaveProfile
    {
        public string id;
        public string name;
        public string lastScene;
        public long updatedAt; // unix ms
    }

    [Serializable]
    public class ProfilesData
    {
        public List<SaveProfile> profiles = new List<SaveProfile>();
        public string activeId;
    }

    public static class SaveSystem
    {
        private static ProfilesData _data;
        private static string Dir => Path.Combine(Application.persistentDataPath, "saves");
        private static string FilePath => Path.Combine(Dir, "profiles.json");

        static void EnsureLoaded()
        {
            if (_data != null) return;
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    _data = JsonUtility.FromJson<ProfilesData>(json);
                }
            }
            catch (Exception e) { Debug.LogWarning($"[SaveSystem] load failed: {e.Message}"); }
            if (_data == null) _data = new ProfilesData();
            if (_data.profiles == null) _data.profiles = new List<SaveProfile>();
            if (string.IsNullOrEmpty(_data.activeId))
            {
                if (_data.profiles.Count == 0)
                {
                    var p = CreateProfile("Player1");
                    _data.activeId = p.id;
                }
                else _data.activeId = _data.profiles[0].id;
            }
        }

        public static IReadOnlyList<SaveProfile> GetProfiles()
        {
            EnsureLoaded();
            return _data.profiles;
        }

        public static SaveProfile GetActive()
        {
            EnsureLoaded();
            foreach (var p in _data.profiles) if (p.id == _data.activeId) return p;
            return _data.profiles.Count > 0 ? _data.profiles[0] : null;
        }

        public static void SetActive(string id)
        {
            EnsureLoaded();
            foreach (var p in _data.profiles) if (p.id == id) { _data.activeId = id; Save(); return; }
        }

        public static SaveProfile CreateProfile(string name)
        {
            EnsureLoaded();
            var p = new SaveProfile { id = System.Guid.NewGuid().ToString("N"), name = string.IsNullOrWhiteSpace(name) ? $"Player{_data.profiles.Count+1}" : name.Trim(), lastScene = string.Empty, updatedAt = Now() };
            _data.profiles.Add(p);
            _data.activeId = p.id;
            Save();
            return p;
        }

        public static void SetLastSceneForActive(string scene)
        {
            EnsureLoaded();
            var p = GetActive(); if (p == null) return;
            p.lastScene = scene; p.updatedAt = Now(); Save();
        }

        public static string GetLastSceneForActive()
        {
            var p = GetActive(); return p != null ? p.lastScene : string.Empty;
        }

        static void Save()
        {
            try
            {
                if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                var json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e) { Debug.LogWarning($"[SaveSystem] save failed: {e.Message}"); }
        }

        static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

