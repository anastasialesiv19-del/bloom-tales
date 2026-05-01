using Match3.Unity.GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class LevelSelectController : MonoBehaviour
    {
        public void OnLevel1Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level1);
            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");
        }

        public void OnLevel2Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level2);
            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");   
        }

        public void OnLevel3Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level3);
            SelectedLevelState.Current = config;
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}