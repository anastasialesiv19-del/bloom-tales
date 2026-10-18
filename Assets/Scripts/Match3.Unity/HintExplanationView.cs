using System;
using TMPro;
using UnityEngine;

namespace Match3.Unity
{
    public class HintExplanationView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text explanationText;

        public void Show(string message)
        {
            if (message == null || message.Trim().Length == 0)
            {
                Hide();
            }
            else
            {
                explanationText.text = message;
                panelRoot.SetActive(true);
            }
        }
        public void Hide()
        {
            explanationText.text = null;
            panelRoot.SetActive(false);
        }
        public void Awake()
        {
           Hide(); 
        }
    }
}