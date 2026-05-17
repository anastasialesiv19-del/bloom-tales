using System.Collections.Generic;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Menu;
using Match3.Unity.Theme;
using UnityEngine;

namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        private void ApplySelectedLevelConfigIfPresent()
        {
            var config = SelectedLevelState.Current;
            if (config == null)
                config = LevelConfigFactory.Create(LevelId.Level1);
            currentLevelId = config.LevelId;
            currentObstacles = config.Obstacles;
            currentGoalDefinitions = config.Goals;
            
            gameMode = config.GameMode;
            totalMoves = config.TotalMoves;
            totalGameSeconds = config.TotalGameSeconds;
            
            SelectedLevelState.Clear();
        }

        private void ApplyTheme(LevelTheme theme)
        {
            if (backgroundImage != null)
                backgroundImage.sprite = theme.background;
        }
        
        private void RefreshResourceText()
        {

            if (gameMode == GameModeType.Timer)
            {
                var totalSeconds = Mathf.CeilToInt(timerController.RemainingSeconds);
                var minutes = totalSeconds / 60;
                var seconds = totalSeconds % 60;
                timerText.text = $"Timer: {minutes:00}:{seconds:00}";
            }
            else
            {
                timerText.text = $"Moves: {moveLimitController.RemainingMoves}";
            }
        }
    }
}