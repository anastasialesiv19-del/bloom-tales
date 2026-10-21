using System;
using System.IO;
using UnityEngine;

namespace Match3.Unity.Settings
{
    public class JsonSettingsStore
    {
        private readonly string filePath;

        public JsonSettingsStore()
        {
            filePath = Path.Combine(Application.persistentDataPath, "settings.json");
        }

        public JsonSettingsStore(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));
            this.filePath = filePath;
        }
        
        public GameSettings Load()
        {
            if (!File.Exists(filePath))
            {
                return new GameSettings();
            }

            var settings = new GameSettings();
            try
            {
                var content = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(content))
                {
                    return new GameSettings();
                }
                
                JsonUtility.FromJsonOverwrite(content, settings);
                settings.Normalize();
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Error reading the settings file: {e.Message}");
                return new GameSettings();
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogWarning($"No access to the settings file: {e.Message}");
                return new GameSettings();
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning($"Invalid JSON data in the settings file: {e.Message}");
                return new GameSettings();
            }
            
            return settings;
        }

        public void Save(GameSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            
            settings.Normalize();
            var settingsToJson = JsonUtility.ToJson(settings, true);
            
            try
            {
                File.WriteAllText(filePath, settingsToJson);
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Error writing the settings file: {e.Message}");
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogWarning($"No access to the settings file: {e.Message}");
            }
        }
    }
    
}