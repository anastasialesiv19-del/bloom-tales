using Core.Progress;
using UnityEngine;

namespace Match3.Unity.Bootstrap
{
    public partial class GameBootstrap
    {
        public void OnHintButtonClick()
        {
            if (_isPaused || _gameOverShown)
            {
                return;
            }
            if (IsCurrentModeExpired())
            {
                return;
            }
            if (turnPlaybackController.IsPlaying)
            {
                return;
            }
            
            _inputController.ClearSelection();
            var effectiveIdleSeconds = Mathf.Max(_idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds, true);
            _hintShownForCurrentIdle = false;
        }

        private void TryShowIdleHint()
        {
            if (!_gameSettings.autoHintsEnabled)
            {
                return;
            }
            if (_isPaused || _gameOverShown)
            {
                return;
            }
            if (IsCurrentModeExpired())
            {
                return;
            }
            if (_hintShownForCurrentIdle)
            {
                return;
            }
            if (_idleTracker.IdleSeconds < idleHintDelaySeconds)
            {
                return;
            }
            
            _inputController.ClearSelection();
            ShowAdaptiveHintInternal(_idleTracker.IdleSeconds, false);
            _hintShownForCurrentIdle = true;
        }
        
        private void ShowAdaptiveHintInternal(float idleSeconds, bool isManualHint)
        {
            if (_isPaused || _gameOverShown)
            {
                return;
            }
            if (IsCurrentModeExpired())
            {
                return;
            }
            _hintController.ClearHint();
            
            var state = new AdaptiveHintState(_consecutiveHintsShown, _totalHintsShown);
            var goalContext = BuildHintGoalContext();
            var shownMove = _hintController.ShowAdaptiveHint(idleSeconds, state, goalContext);
            _idleTracker.ResetIdle();

            if (shownMove.HasValue)
            {
                _activeHintMove = shownMove.Value;
                _consecutiveHintsShown++;
                _totalHintsShown++;

                if (isManualHint)
                {
                    _manualHintsShownThisRun++;
                }
                else
                {
                    _idleHintsShownThisRun++;
                }
                SaveProgress();
            }
            else
            {
                _consecutiveHintsShown = 0;
                _activeHintMove = null;
            }
        }
    }
}