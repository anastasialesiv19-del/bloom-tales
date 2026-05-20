using Match3.Unity.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private StatsPanelController statsPanel;
        
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
    }
}