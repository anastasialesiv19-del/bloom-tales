using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Matching;
using Match3.Unity.Goals;
using UnityEngine;

namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        private void RebuildGoals()
        {
            goals = new List<IGoal>();

            foreach (var def in currentGoalDefinitions)
            {
                goals.Add(GoalFactory.Create(def));
            }
        }

        private void RebuildGoalViews()
        {
            foreach (Transform child in goalsRoot)
            {
                Destroy(child.gameObject);
            }

            goalViews.Clear();

            foreach (var goal in goals)
            {
                Sprite icon = null;

                switch (goal)
                {
                    case ColorGoalProgress colorGoal:
                        icon = colorGoal.Color switch
                        {
                            PieceColor.Red => redGoalIcon,
                            PieceColor.Blue => blueGoalIcon,
                            PieceColor.Green => greenGoalIcon,
                            _ => null
                        };
                        break;

                    case ObstacleGoalProgress obstacleGoal:
                        icon = obstacleGoal.Kind switch
                        {
                            ObstacleKind.Box => boxGoalIcon,
                            _ => null
                        };
                        break;

                    default:
                        continue;
                }

                var view = Instantiate(goalItemPrefab, goalsRoot);
                view.Bind(goal, icon);
                goalViews.Add(view);
            }
        }

        private void RefreshGoalHud()
        {
            var scoreGoal = GetScoreGoal();

            scoreText.text = scoreGoal.Current.ToString();
            targetText.text = $"Target: {scoreGoal.Target}";

            float progress = Mathf.Clamp01((float)scoreGoal.Current / scoreGoal.Target);
            progressBar.fillAmount = progress;
            progressBar.color = Color.Lerp(Color.red, Color.green, progress);

            foreach (var view in goalViews)
            {
                view.Refresh();
            }
        }

        private GoalProgress GetScoreGoal()
        {
            return (GoalProgress)goals.First(g => g.Type == GoalType.Score);
        }
    }
}