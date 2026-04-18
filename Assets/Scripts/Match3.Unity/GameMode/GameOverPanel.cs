using TMPro;
using UnityEngine;

namespace Match3.Unity.GameMode
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text messageText;
        
        public void Show()
        {
            if (messageText != null)
                messageText.text = "Time is up";
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}