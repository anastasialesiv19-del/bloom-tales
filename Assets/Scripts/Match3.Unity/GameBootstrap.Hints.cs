using Core.Progress;
using UnityEngine;

namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        private void TryShowIdleHint()
        {
            if (isPaused) return;
            if (IsCurrentModeExpired()) return;
            if (hintShownForCurrentIdle) return;
            if (idleTracker.IdleSeconds < idleHintDelaySeconds) return;
            ShowAdaptiveHintInternal(idleTracker.IdleSeconds, false);
            hintShownForCurrentIdle = true;
        }
        
        public void OnHintButtonClick()
        {
            if (isPaused) return;
            if(IsCurrentModeExpired()) return;
            if (turnPlaybackController.IsPlaying) return;
            var effectiveIdleSeconds = Mathf.Max(idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds, true);
            hintShownForCurrentIdle = false;
        }

        private void ShowAdaptiveHintInternal(float idleSeconds, bool isManualHint)
        {
            if (isPaused) return;
            if(IsCurrentModeExpired()) return;
            hintController.ClearHint();
            
            var state = new AdaptiveHintState(consecutiveHintsShown, totalHintsShown);
            var shownMove = hintController.ShowAdaptiveHint(idleSeconds, state);
            idleTracker.ResetIdle();

            if (shownMove.HasValue)
            {
                activeHintMove = shownMove.Value;
                consecutiveHintsShown++;
                totalHintsShown++;

                if (isManualHint)
                    manualHintsShownThisRun++;
                else
                    idleHintsShownThisRun++;
                SaveProgress();
            }
            else
            {
                consecutiveHintsShown = 0;
                activeHintMove = null;
            }
        }
    }
}