using System;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Hint;
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
                goals.Add(GoalFactory.Create(def));
        }

        private void RebuildGoalViews()
        {
            foreach (Transform child in goalsRoot)
                Destroy(child.gameObject);
            
            goalViews.Clear();
            foreach (var goal in goals)
            {
                Sprite icon = null;

                switch (goal)
                {
                    case ColorGoalProgress colorGoal:
                        icon = pieceSpriteLibrary.GetSprite(Piece.Normal(colorGoal.Color));
                        break;
                    case ObstacleGoalProgress obstacleGoal:
                        icon = obstacleGoal.Kind switch
                        {
                            ObstacleKind.Box => boxGoalIcon,
                            _ => null
                        };
                        break;
                    case GoalProgress scoreGoal:
                        icon = scoreGoalIcon;
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
            scoreText.text = $"Score: {scoreThisRun}";
            
            foreach (var view in goalViews)
            {
                view.Refresh();
            }
        }
        
        private HintGoalContext BuildHintGoalContext()
        {
            var remainingByColor = new Dictionary<PieceColor, int>();
            var remainingObstacles = 0;
            var remainingScore = 0;
            foreach (var goal in goals)
            {
                switch (goal)
                {
                    case ColorGoalProgress colorGoal:
                    {
                        var remaining = Mathf.Max(0, colorGoal.Target - colorGoal.Current);
                        if (remaining > 0) remainingByColor[colorGoal.Color] = remaining;
                        break;
                    }
                    case ObstacleGoalProgress obstacleGoal:
                        remainingObstacles += Mathf.Max(0, obstacleGoal.Target - obstacleGoal.Current);
                        break;
                    case GoalProgress scoreGoal:
                        remainingScore = Mathf.Max(0, scoreGoal.Target - scoreGoal.Current);
                        break;
                }
            }
            return new HintGoalContext(remainingByColor, remainingObstacles, remainingScore);
        }
    }
}