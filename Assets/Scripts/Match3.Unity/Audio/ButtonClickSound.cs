using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Unity.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour, IPointerDownHandler
    {
        private Button _button;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (!_button.IsActive() || !_button.IsInteractable())
            {
                return;
            }

            if (GameAudioController.Instance != null)
            {
                GameAudioController.Instance.PlayButtonClick();
            }
        }
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        
    }
}