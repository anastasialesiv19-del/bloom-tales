using Match3.Unity.Audio;
using Match3.Unity.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class LevelSelectController : MonoBehaviour
    {
        private void Start()
        {
            GameAudioController.Instance.PlayMenuMusic();
        }
        
        public void OnLevel1Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level1);
            SelectedLevelState.Set(config);
            SceneManager.LoadScene("SampleScene");
        }

        public void OnLevel2Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level2);
            SelectedLevelState.Set(config);
            SceneManager.LoadScene("SampleScene");   
        }

        public void OnLevel3Clicked()
        {
            var config = LevelConfigFactory.Create(LevelId.Level3);
            SelectedLevelState.Set(config);
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnBackClicked()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}