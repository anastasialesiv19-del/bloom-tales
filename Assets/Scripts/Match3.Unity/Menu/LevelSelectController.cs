using Match3.Unity.GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class LevelSelectController : MonoBehaviour
    {
        public void OnLevel1Clicked()
        {
            var config = new LevelConfig(
                "Level 1",
                GameModeType.Moves,
                40,
                0f);

            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");
        }

        public void OnLevel2Clicked()
        {
            var config = new LevelConfig(
                "Level 2",
                GameModeType.Timer,
                0,
                90f);

            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");   
        }

        public void OnLevel3Clicked()
        {
            
        }
        
        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}