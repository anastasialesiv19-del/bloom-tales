using Core.Progress;
using UnityEngine;

namespace Match3.Unity
{
    public partial class GameBootstrap
    {
        private void TryShowIdleHint()
        {
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
            if (hintShownForCurrentIdle) return;
            if (idleTracker.IdleSeconds < idleHintDelaySeconds) return;
            ShowAdaptiveHintInternal(idleTracker.IdleSeconds);
            hintShownForCurrentIdle = true;
        }
        
        public void OnHintButtonClick()
        {
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
            if (turnPlaybackController.IsPlaying) return;
            var effectiveIdleSeconds = Mathf.Max(idleTracker.IdleSeconds, idleHintDelaySeconds);
            ShowAdaptiveHintInternal(effectiveIdleSeconds);
            hintShownForCurrentIdle = false;
        }

        private void ShowAdaptiveHintInternal(float idleSeconds)
        {
            if (isPaused)
                return;
            if(IsCurrentModeExpired()) return;
            hintController.ClearHint();
            
            var state = new AdaptiveHintState(consecutiveHintsShown, totalHintsShown);
            var wasShown = hintController.ShowAdaptiveHint(idleSeconds, state);
            idleTracker.ResetIdle();

            if (wasShown)
            {
                consecutiveHintsShown++;
                totalHintsShown++;
                SaveProgress();
            }
            else
                consecutiveHintsShown = 0;
        }
    }
}