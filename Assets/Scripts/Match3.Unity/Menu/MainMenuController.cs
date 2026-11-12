using Match3.Unity.Audio;
using Match3.Unity.Settings;
using Match3.Unity.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private StatsPanelController statsPanel;
        [SerializeField] private SettingsPanelView settingsPanel;
        private JsonSettingsStore _settingsStore;
        private GameSettings _gameSettings;

        private void Start()
        {
            GameAudioController.Instance.PlayMenuMusic();
        }

        private void Awake()
        {
            _settingsStore = new JsonSettingsStore();
            _gameSettings = _settingsStore.Load();
        }
        
        public void OnPlayClicked()
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
        
        public void OnExitGameClicked()
        {
            Application.Quit();
        }
        
        public void OnStatsClicked()
        {
            if (statsPanel != null)
            {
                statsPanel.Show();
            }
        }

        public void OnSettingsClicked()
        {
            if (settingsPanel != null)
            {
                settingsPanel.Show(_gameSettings);
            }
        }

        public void OnSettingsBackClicked()
        {
            _settingsStore.Save(_gameSettings);
            settingsPanel.Hide();
        }
    }
}