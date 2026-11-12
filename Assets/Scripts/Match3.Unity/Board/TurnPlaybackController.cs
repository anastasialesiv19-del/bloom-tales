using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Board;
using Core.Contracts;
using Core.Presentation;
using Core.Turns;
using Match3.Unity.Audio;
using UnityEngine;

namespace Match3.Unity.Board
{
    public class TurnPlaybackController : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private float stepDelaySeconds = 0.4f;
        [SerializeField] private float activationChainDelaySeconds = 0.1f;
        [SerializeField, Min(0f)] private float pieceFadeSeconds = 0.15f;
        [SerializeField, Min(0f)] private float swapDurationSeconds = 0.3f;
        [SerializeField, Min(0f)] private float fallDurationSeconds = 0.35f;
        public bool IsPlaying { get; private set; }

        public void Play(TurnPresentationModel presentation)
        {
            if (presentation == null)
            {
                throw new ArgumentNullException(nameof(presentation));
            }
            StartCoroutine(PlayRoutine(presentation));
        }

        private IEnumerator PlayRoutine(TurnPresentationModel presentation)
        {
            IsPlaying = true;

            foreach (var phase in presentation.Phases)
            {
                if (phase.BoardAfterPhase == null)
                {
                    continue;
                }
                
                var soundsCompleted = false;
                StartCoroutine(PlayPhaseSounds(phase.Type, phase.ClearedCells, phase.SpecialSpawns.Count,
                    phase.SpecialActivations.Select(activation => activation.ChainDepth), () => soundsCompleted = true));
                
                if (phase.Type == TurnPhaseType.SwapApplied && presentation.Move.HasValue)
                {
                    yield return boardRenderer.RenderSwap(phase.BoardAfterPhase, presentation.Move.Value, swapDurationSeconds);
                }
                else if (phase.Type == TurnPhaseType.GravityApplied)
                {
                    yield return boardRenderer.RenderGravity(phase.BoardAfterPhase, phase.Falls, fallDurationSeconds);
                }
                else if (phase.Type == TurnPhaseType.CascadeWaveResolved && phase.CascadeAnimation != null)
                {
                    var animations = phase.CascadeAnimation;

                    yield return boardRenderer.RenderAnimated(animations.AfterClear, pieceFadeSeconds);
                    yield return boardRenderer.RenderGravity(animations.AfterGravity, animations.Falls, fallDurationSeconds);
                    yield return boardRenderer.RenderAnimated(phase.BoardAfterPhase, pieceFadeSeconds);
                }
                else
                {
                    yield return boardRenderer.RenderAnimated(phase.BoardAfterPhase, pieceFadeSeconds);
                }
                
                while (!soundsCompleted)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(stepDelaySeconds);
            }
            boardRenderer.Render(presentation.FinalBoard);
            IsPlaying = false;
        }
        
        private IEnumerator PlayPhaseSounds(TurnPhaseType phaseType, int clearedCells, int spawnCount, IEnumerable<int> activationDepths, Action onCompleted)
        {
            try
            {
                var audio = GameAudioController.Instance;
                if (audio == null)
                {
                    yield break;
                }

                if ((phaseType == TurnPhaseType.PrimaryMatchResolved || phaseType == TurnPhaseType.CascadeWaveResolved) && clearedCells > 0)
                {
                    audio.PlayMatchSound();
                }

                foreach (var unused in Enumerable.Range(0, spawnCount))
                {
                    audio.PlaySpecialSpawnSound();
                }

                var groups = activationDepths.GroupBy(depth => depth).OrderBy(group => group.Key);
                var firstGroup = true;
                foreach (var group in groups)
                {
                    if (!firstGroup)
                    {
                        yield return new WaitForSeconds(activationChainDelaySeconds);
                    }

                    if (audio == null)
                    {
                        yield break;
                    }

                    foreach (var unused in group)
                    {
                        audio.PlaySpecialActivationSound();
                    }
                    
                    firstGroup = false;
                }
            }
            finally
            {
                onCompleted();
            }
        }
    }
}