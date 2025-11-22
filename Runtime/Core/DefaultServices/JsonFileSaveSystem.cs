using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Core.Utils.Logs.Logger;

namespace Core.DefaultServices
{
    /// <summary>
    /// JSON file-based save system using Newtonsoft.Json.
    /// Saves each slot as a separate JSON file under Application.persistentDataPath.
    /// </summary>
    public class JsonFileSaveSystem : ISaveSystem
    {
        private readonly string _rootFolder;
        private readonly int _version;

        /// <summary>
        /// Creates a new JSON file-based save system.
        /// </summary>
        /// <param name="folderName">Folder name under persistent data path where saves are stored.</param>
        /// <param name="version">Current save data version. Used for migration checks.</param>
        public JsonFileSaveSystem(string folderName = "Saves", int version = 1)
        {
            _version = version;
            _rootFolder = Path.Combine(Application.persistentDataPath, folderName);

            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public void Save<T>(string slot, T data)
        {
            if (string.IsNullOrEmpty(slot))
            {
                Logger.LogError("[SaveSystem] Save called with null/empty slot.");
                return;
            }

            if (data == null)
            {
                Logger.LogError($"[SaveSystem] Save called for slot '{slot}' with null data.");
                return;
            }

            try
            {
                var container = new SaveContainer<T>
                {
                    Version = _version,
                    Type = typeof(T).AssemblyQualifiedName,
                    Payload = data
                };

                string json = JsonConvert.SerializeObject(
                    container,
                    Formatting.Indented // nice for debugging
                );

                string path = GetPathForSlot(slot);
                File.WriteAllText(path, json);
                
                Logger.Log($"[SaveSystem] Saved slot '{slot}' to: {path}");
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to save slot '{slot}': {e}");
            }
        }

        public T Load<T>(string slot, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(slot))
            {
                Logger.LogError("[SaveSystem] Load called with null/empty slot.");
                return defaultValue;
            }

            string path = GetPathForSlot(slot);

            if (!File.Exists(path))
            {
                Logger.Log($"[SaveSystem] No save found for slot '{slot}'. Returning default value.");
                return defaultValue;
            }

            try
            {
                string json = File.ReadAllText(path);
                var container = JsonConvert.DeserializeObject<SaveContainer<T>>(json);

                if (container == null)
                {
                    Logger.LogWarning($"[SaveSystem] Failed to parse save for slot '{slot}'. Returning default.");
                    return defaultValue;
                }

                // Version check hook – migrations can be handled here later
                if (container.Version != _version)
                {
                    Logger.LogWarning(
                        $"[SaveSystem] Save version mismatch for slot '{slot}'. " +
                        $"Save: {container.Version}, Current: {_version}. Returning payload as-is."
                    );
                }

                return container.Payload != null ? container.Payload : defaultValue;
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to load slot '{slot}': {e}");
                return defaultValue;
            }
        }

        public bool HasSave(string slot)
        {
            if (string.IsNullOrEmpty(slot))
                return false;

            string path = GetPathForSlot(slot);
            return File.Exists(path);
        }

        /// <summary>
        /// Asynchronously saves data to a slot by delegating to the synchronous <see cref="Save{T}"/>.
        /// </summary>
        public Task SaveAsync<T>(string slot, T data)
        {
            // For now just wrap the sync implementation; safe because it does only file I/O and logging.
            return Task.Run(() => Save(slot, data));
        }

        /// <summary>
        /// Asynchronously loads data from a slot by delegating to the synchronous <see cref="Load{T}"/>.
        /// </summary>
        public Task<T> LoadAsync<T>(string slot, T defaultValue = default)
        {
            // Same as above: wrap existing sync implementation.
            return Task.Run(() => Load(slot, defaultValue));
        }

        /// <summary>
        /// Returns the version stored in the save container for the specified slot, or -1 if missing/invalid.
        /// </summary>
        public int GetVersion(string slot)
        {
            if (string.IsNullOrEmpty(slot))
                return -1;

            string path = GetPathForSlot(slot);
            if (!File.Exists(path))
                return -1;

            try
            {
                string json = File.ReadAllText(path);
                // We don't know T here, so deserialize into a non-generic container with only version.
                var versionOnly = JsonConvert.DeserializeObject<VersionContainer>(json);
                return versionOnly?.Version ?? -1;
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to read version for slot '{slot}': {e}");
                return -1;
            }
        }

        /// <summary>
        /// Deletes the JSON file for the specified slot, if it exists.
        /// </summary>
        public void Delete(string slot)
        {
            if (string.IsNullOrEmpty(slot))
                return;

            string path = GetPathForSlot(slot);
            if (!File.Exists(path))
                return;

            try
            {
                File.Delete(path);
                Logger.Log($"[SaveSystem] Deleted save slot '{slot}'.");
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to delete slot '{slot}': {e}");
            }
        }

        /// <summary>
        /// Deletes all JSON save files in the configured root folder.
        /// </summary>
        public void DeleteAll()
        {
            if (!Directory.Exists(_rootFolder))
                return;

            try
            {
                var files = Directory.GetFiles(_rootFolder, "*.json");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                
                Logger.Log("[SaveSystem] Deleted all save slots.");
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to delete all saves: {e}");
            }
        }

        /// <summary>
        /// Returns all slot names (without extension) for existing JSON save files.
        /// </summary>
        public IEnumerable<string> GetAllSlots()
        {
            if (!Directory.Exists(_rootFolder))
                return Array.Empty<string>();

            try
            {
                return Directory
                    .GetFiles(_rootFolder, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError($"[SaveSystem] Failed to enumerate save slots: {e}");
                return Array.Empty<string>();
            }
        }

        private string GetPathForSlot(string slot)
        {
            var safeSlot = MakeFileNameSafe(slot);
            return Path.Combine(_rootFolder, safeSlot + ".json");
        }

        private static string MakeFileNameSafe(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        [Serializable]
        private class SaveContainer<T>
        {
            [JsonProperty("version")]
            public int Version;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("payload")]
            public T Payload;
        }

        /// <summary>
        /// Lightweight container used only for reading the version from a save file.
        /// </summary>
        [Serializable]
        private class VersionContainer
        {
            [JsonProperty("version")]
            public int Version;
        }
    }
}