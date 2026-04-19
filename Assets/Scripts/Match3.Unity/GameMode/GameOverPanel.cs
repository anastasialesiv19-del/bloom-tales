using TMPro;
using UnityEngine;

namespace Match3.Unity.GameMode
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text messageText;
        
        public void Show(string message)
        {
            if (messageText != null)
                messageText.text = message;
            if(root != null)
                root.SetActive(true);
        }

        public void Hide()
        {
            if(root != null)
                root.SetActive(false);
        }
    }
}