using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Unity.Audio
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour, IPointerDownHandler
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!button.IsActive() || !button.IsInteractable())
                return;

            if (GameAudioController.Instance != null)
                GameAudioController.Instance.PlayButtonClick();
        }
    }
}