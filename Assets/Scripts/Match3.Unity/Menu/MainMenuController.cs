using Match3.Unity.Audio;
using Match3.Unity.Save;
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
        private JsonSettingsStore settingsStore;
        private GameSettings gameSettings;

        private void Start()
        {
            GameAudioController.Instance.PlayMenuMusic();
        }

        private void Awake()
        {
            settingsStore = new JsonSettingsStore();
            gameSettings = settingsStore.Load();
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
                statsPanel.Show();
        }

        public void OnSettingsClicked()
        {
            if (settingsPanel != null)
                settingsPanel.Show(gameSettings);
        }

        public void OnSettingsBackClicked()
        {
            settingsStore.Save(gameSettings);
            settingsPanel.Hide();
        }
    }
}