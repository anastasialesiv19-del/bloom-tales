using System.Collections.Generic;
using Core.Matching;
using Match3.Unity.GameFlow;
using Match3.Unity.GameMode;
using Match3.Unity.Goals;
using Match3.Unity.Menu;
using Match3.Unity.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        public void OnRestartButtonClick()
        {
            if (isPaused)
            {
                pausePanel.Hide();
                isPaused = false;
            }
            StartNewGameFlow();
        }

        public void OnNextLevelClicked()
        {
            if (!LevelProgression.TryGetNext(currentLevelId, out var nextLevel))
            {
                OnBackToMenuClicked();
                return;
            }
            var config = LevelConfigFactory.Create(nextLevel);;
            SelectedLevelState.Set(config);
            SceneManager.LoadScene("SampleScene");
        }
        
        public void OnBackToMenuClicked()
        {
            hintController.ClearHint();
            inputController.ClearSelection();
            pausePanel.Hide();
            isPaused = false;
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;

            SelectedLevelState.Clear();
            SceneManager.LoadScene("MainMenuScene");
        }
        
        private void StartNewGameFlow()
        {
            hintController.ClearHint();
            inputController.ClearSelection();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            consecutiveHintsShown = 0;
            
            ResetRunStats();
            RebuildGoals();
            RebuildGoalViews();
            RefreshGoalHud();
            
            var snapshot = session.StartNewGame(startHeight, startWidth, currentObstacles);
            boardRenderer.RebuildAndRender(snapshot);
            
            if (gameMode == GameModeType.Timer)
            {
                timerController.StartTimer(totalGameSeconds);
            }
            else
            {
                moveLimitController.ResetMoves(totalMoves);
            }

            RefreshResourceText();
            
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
            gameOverShown = false;
        }
        
        private void HandleGameResult(GameResultType result)
        {
            SaveLevelResult(result);
            hintController.ClearHint();
            inputController.ClearSelection();

            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            if (defeatPanel != null)
                defeatPanel.SetActive(false);

            switch (result)
            {
                case GameResultType.Defeat:
                    defeatPanel.SetActive(true);
                    break;

                case GameResultType.Victory:
                    victoryPanel.SetActive(true);
                    break;
            }
        }
        
        private bool IsCurrentModeExpired()
        {
            return gameMode == GameModeType.Timer ? timerController.IsExpired : moveLimitController.IsExpired;
        }
        
        private void ResetRunStats()
        {
            manualHintsShownThisRun = 0;
            idleHintsShownThisRun = 0;
            hintsAppliedThisRun = 0;

            clearedCellsThisRun = 0;
            destroyedObstaclesThisRun = 0;
            turnsUsedThisRun = 0;

            activeHintMove = null;
            resultSavedForCurrentRun = false;
        }
        private void SaveLevelResult(GameResultType result)
        {
            if (resultSavedForCurrentRun) return;

            resultSavedForCurrentRun = true;

            levelResultStore.Append(new LevelRunResultDto
            {
                LevelId = currentLevelId.ToString(),
                Result = result.ToString(),
                TurnsUsed = turnsUsedThisRun,
                ManualHintsShown = manualHintsShownThisRun,
                IdleHintsShown = idleHintsShownThisRun,
                HintsApplied = hintsAppliedThisRun,
                TotalClearedCells = clearedCellsThisRun,
                DestroyedObstacles = destroyedObstaclesThisRun,
                CompletedAtUtc = System.DateTime.UtcNow.ToString("O")
            });
        }
    }
}