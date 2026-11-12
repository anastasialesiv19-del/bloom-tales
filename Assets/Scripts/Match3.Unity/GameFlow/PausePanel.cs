using UnityEngine;

namespace Match3.Unity.GameFlow
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
       
        public void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}