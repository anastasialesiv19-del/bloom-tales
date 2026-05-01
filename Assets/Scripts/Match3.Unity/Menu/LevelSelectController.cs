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
                LevelId.Level1,
                "Level 1",
                GameModeType.Moves,
                3,
                0f);

            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");
        }

        public void OnLevel2Clicked()
        {
            var config = new LevelConfig(
                LevelId.Level2,
                "Level 2",
                GameModeType.Timer,
                0,
                10f);

            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");   
        }

        public void OnLevel3Clicked()
        {
            var config = new LevelConfig(
                LevelId.Level3,
                "Level 3",
                GameModeType.Moves,
                5,
                0f);
            
            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}