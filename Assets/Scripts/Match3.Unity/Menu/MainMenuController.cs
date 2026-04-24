using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity.Menu
{
    public class MainMenuController : MonoBehaviour
    {
        public void OnPlayClicked()
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
        
        public void OnExitGameClicked()
        {
            Application.Quit();
        }
    }
}