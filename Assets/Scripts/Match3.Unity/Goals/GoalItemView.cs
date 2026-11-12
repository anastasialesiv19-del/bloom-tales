using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Unity.Goals
{
    public class GoalItemView :MonoBehaviour
    {
        [SerializeField] private Image goalIcon;
        [SerializeField] private TMP_Text goalText;
        [SerializeField] private Image completedIcon;
        private IGoal _boundGoal;

        public void Bind(IGoal goal, Sprite iconSprite)
        {
            _boundGoal = goal;
            
            if (goalIcon != null)
            {
                goalIcon.sprite = iconSprite;
            }
            Refresh();
        }

        public void Refresh()
        {
            if (_boundGoal == null)
            {
                return;
            }

            goalText.text = $"{Mathf.Min(GetCurrent(_boundGoal), GetTarget(_boundGoal))} / {GetTarget(_boundGoal)}";

            if (_boundGoal.IsCompleted)
            {
                completedIcon.gameObject.SetActive(true);
            }
        }
        
        private static int GetCurrent(IGoal goal)
        {
            switch (goal)
            {
                case ColorGoalProgress colorGoal:
                    return colorGoal.Current;
                case ObstacleGoalProgress obstacleGoal:
                    return obstacleGoal.Current;
                case GoalProgress scoreGoal:
                    return scoreGoal.Current;
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
                case GoalProgress scoreGoal:
                    return scoreGoal.Target;
                default:
                    return 0;
            }
        }   
    }
}