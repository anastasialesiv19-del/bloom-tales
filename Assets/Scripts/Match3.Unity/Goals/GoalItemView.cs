using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Goals
{
    public class GoalItemView :MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        private ColorGoalProgress boundGoal;

        public void Bind(ColorGoalProgress goal, Sprite iconSprite)
        {
            boundGoal = goal;

            if (icon != null)
                icon.sprite = iconSprite;

            Refresh();
        }

        public void Refresh()
        {
            if (boundGoal == null) return;

            text.text = $"{boundGoal.Current}  /  {boundGoal.Target}";
        }
    }
}