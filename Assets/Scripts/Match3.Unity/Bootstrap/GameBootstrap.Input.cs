namespace Match3.Unity.Bootstrap
{
    public partial class GameBootstrap
    {
        public void OnPauseButtonClick()
        {
            if (turnPlaybackController.IsPlaying)
            {
                return;
            }
            if (IsCurrentModeExpired())
            {
                return;
            }
            
            _hintController.ClearHint();
            _inputController.ClearSelection();
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;
            
            pausePanel.Show();
            _isPaused = true;
        }

        public void OnResumeButtonClick()
        {
            pausePanel.Hide();
            _isPaused = false;
        }

        public void OnSettingsButtonClick()
        {
            if (!_isPaused || _gameOverShown)
            {
                return;
            }
            pausePanel.Hide();
            settingsPanelView.Show(_gameSettings);
        }

        public void OnSettingsBackButtonClick()
        {
            _settingsStore.Save(_gameSettings);
            settingsPanelView.Hide();
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;
            pausePanel.Show();
        }
        
        private void HandleBoardCellClicked(int row, int col)
        {
            _hintController.ClearHint();
            _idleTracker.ResetIdle();
            _hintShownForCurrentIdle = false;
        }
    }
}