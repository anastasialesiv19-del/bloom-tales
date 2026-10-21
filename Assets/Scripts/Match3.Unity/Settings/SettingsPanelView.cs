using Match3.Unity.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Settings
{
    public class SettingsPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Slider musicSlider, effectsSlider;
        [SerializeField] private Toggle autoHintsToggle;
        private GameSettings settings;

        public void Show(GameSettings settings)
        {
            this.settings = settings;
            musicSlider.SetValueWithoutNotify(settings.musicVolume);
            effectsSlider.SetValueWithoutNotify(settings.effectVolume);
            autoHintsToggle.SetIsOnWithoutNotify(settings.autoHintsEnabled);
            panelRoot.SetActive(true);
        }

        public void Hide()
        {
            panelRoot.SetActive(false);
        }

        public void Awake()
        {
            Hide();
        }

        public void OnMusicVolumeChanged(float value)
        {
            if (settings == null) return;
            
            settings.musicVolume = Mathf.Clamp01(value);
            GameAudioController.Instance.SetMusicVolume(settings.musicVolume);
        }

        public void OnEffectsVolumeChanged(float value)
        {
            if (settings == null) return;
            
            settings.effectVolume = Mathf.Clamp01(value);
            GameAudioController.Instance.SetEffectsVolume(settings.effectVolume);
        }

        public void OnAutoHintsChanged(bool value)
        {
            if (settings != null) settings.autoHintsEnabled = value;
        }
    }
}