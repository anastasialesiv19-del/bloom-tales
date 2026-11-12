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
        private GameSettings _settings;

        public void Show(GameSettings settings)
        {
            _settings = settings;
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
            if (_settings == null)
            {
                return;
            }
            
            _settings.musicVolume = Mathf.Clamp01(value);
            GameAudioController.Instance.SetMusicVolume(_settings.musicVolume);
        }

        public void OnEffectsVolumeChanged(float value)
        {
            if (_settings == null)
            {
                return;
            }
            
            _settings.effectVolume = Mathf.Clamp01(value);
            GameAudioController.Instance.SetEffectsVolume(_settings.effectVolume);
        }

        public void OnAutoHintsChanged(bool value)
        {
            if (_settings != null)
            {
                _settings.autoHintsEnabled = value;
            }
        }
    }
}