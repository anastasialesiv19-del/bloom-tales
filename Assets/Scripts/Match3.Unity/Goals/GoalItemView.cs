using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Goals
{
    public class GoalItemView :MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        private IGoal boundGoal;

        public void Bind(IGoal goal, Sprite iconSprite)
        {
            boundGoal = goal;

            if (icon != null)
                icon.sprite = iconSprite;

            Refresh();
        }

        public void Refresh()
        {
            if (boundGoal == null)
                return;

            text.text = $"{GetCurrent(boundGoal)} / {GetTarget(boundGoal)}";
        }
        
        private static int GetCurrent(IGoal goal)
        {
            switch (goal)
            {
                case ColorGoalProgress colorGoal:
                    return colorGoal.Current;

                case ObstacleGoalProgress obstacleGoal:
                    return obstacleGoal.Current;

                default:
                    return 0;
            }
        }

        private static int GetTarget(IGoal goal)
        {
            switch (goal)
            {
                case ColorGoalProgress colorGoal:
                    return colorGoal.Target;

                case ObstacleGoalProgress obstacleGoal:
                    return obstacleGoal.Target;

                default:
                    return 0;
            }
        }   
    }
}