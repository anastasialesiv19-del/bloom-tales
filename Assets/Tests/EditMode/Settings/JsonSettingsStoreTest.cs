using System;
using System.IO;
using System.Text.RegularExpressions;
using Match3.Unity.Settings;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode.Settings
{
    public class JsonSettingsStoreTest
    {
        [Test]
        public void SaveThenLoad_PreservesSettings()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
            try
            {
                var gameSettings = new GameSettings { musicVolume = 0.2f, effectVolume = 0.8f, autoHintsEnabled = false };

                var jsonSettingsStoreToSave = new JsonSettingsStore(filePath);
                jsonSettingsStoreToSave.Save(gameSettings);

                Assert.IsTrue(File.Exists(filePath));

                var jsonSettingsStoreToLoad = new JsonSettingsStore(filePath);
                var settings = jsonSettingsStoreToLoad.Load();

                Assert.That(settings.musicVolume, Is.EqualTo(gameSettings.musicVolume).Within(0.0001f));
                Assert.That(settings.effectVolume, Is.EqualTo(gameSettings.effectVolume).Within(0.0001f));
                Assert.IsFalse(settings.autoHintsEnabled);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Test]
        public void Load_WhenFieldsAreMissing_PreservesDefaults()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
            try
            {
                File.WriteAllText(filePath, "{\"musicVolume\": 0.2}");
                Assert.IsTrue(File.Exists(filePath));

                var defaultSettings = new GameSettings();

                var jsonSettingsStoreToLoad = new JsonSettingsStore(filePath);
                var settings = jsonSettingsStoreToLoad.Load();

                Assert.That(settings.musicVolume, Is.EqualTo(0.2).Within(0.0001f));
                Assert.That(settings.effectVolume, Is.EqualTo(defaultSettings.effectVolume).Within(0.0001f));
                Assert.IsTrue(settings.autoHintsEnabled);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
        
        [Test]
        public void Load_WhenJsonIsInvalid_ReturnsDefault()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
            try
            {
                File.WriteAllText(filePath, "{\"musicVolume\":}");
                Assert.IsTrue(File.Exists(filePath));
                LogAssert.Expect(LogType.Warning, new Regex(@"^Invalid JSON data in the settings file:"));
                
                var defaultSettings = new GameSettings();

                var jsonSettingsStoreToLoad = new JsonSettingsStore(filePath);
                var settings = jsonSettingsStoreToLoad.Load();

                Assert.That(settings.musicVolume, Is.EqualTo(defaultSettings.musicVolume).Within(0.0001f));
                Assert.That(settings.effectVolume, Is.EqualTo(defaultSettings.effectVolume).Within(0.0001f));
                Assert.IsTrue(settings.autoHintsEnabled);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}