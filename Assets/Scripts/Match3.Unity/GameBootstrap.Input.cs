namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        private void HandleBoardCellClicked(int row, int col)
        {
            hintController.ClearHint();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
        }
        public void OnPauseButtonClick()
        {
            if (turnPlaybackController.IsPlaying)
                return;
            if (IsCurrentModeExpired())
                return;
            
            hintController.ClearHint();
            inputController.ClearSelection();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            
            pausePanel.Show();
            isPaused = true;
        }

        public void OnResumeButtonClick()
        {
            pausePanel.Hide();
            isPaused = false;
        }

        public void OnSettingsButtonClick()
        {
            if (!isPaused || gameOverShown) return;
            pausePanel.Hide();
            settingsPanelView.Show(gameSettings);
        }

        public void OnSettingsBackButtonClick()
        {
            settingsStore.Save(gameSettings);
            settingsPanelView.Hide();
            idleTracker.ResetIdle();
            hintShownForCurrentIdle = false;
            pausePanel.Show();
        }
    }
}