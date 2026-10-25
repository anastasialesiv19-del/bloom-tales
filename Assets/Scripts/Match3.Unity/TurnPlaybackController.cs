using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Contracts;
using Core.Matching;
using Core.Presentation;
using Core.Turns;
using Match3.Unity.Audio;
using UnityEngine;

namespace Match3.Unity
{
    public class TurnPlaybackController : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private float stepDelaySeconds = 0.4f;
        [SerializeField] private float activationChainDelaySeconds = 0.1f;
        public bool IsPlaying { get; private set; }

        public void Play(TurnPresentationModel presentation)
        {
            if (presentation == null) throw new ArgumentNullException(nameof(presentation));
            StartCoroutine(PlayRoutine(presentation));
        }

        private IEnumerator PlayRoutine(TurnPresentationModel presentation)
        {
            IsPlaying = true;

            foreach (var phase in presentation.Phases)
            {
                if (phase.BoardAfterPhase == null) continue;

                boardRenderer.Render(phase.BoardAfterPhase);

                if (GameAudioController.Instance != null)
                {

                    if ((phase.Type == TurnPhaseType.PrimaryMatchResolved || phase.Type == TurnPhaseType.CascadeWaveResolved)
                             && phase.ClearedCells > 0 )
                        GameAudioController.Instance.PlayMatchSound();
                    var specialActivationGroups = phase.SpecialActivations
                        .GroupBy(phases => phases.ChainDepth).OrderBy(events => events.Key);

                    int groupIndex = 0;
                    foreach (var specialActivations in specialActivationGroups)
                    {
                        if (groupIndex != 0)
                             yield return new WaitForSeconds(activationChainDelaySeconds);
                        foreach (var specialActivation in specialActivations)
                        {
                            GameAudioController.Instance.PlaySpecialActivationSound();
                        }
                        groupIndex++;
                    }
                        
                    foreach (var spawn in phase.SpecialSpawns) 
                    {
                        GameAudioController.Instance.PlaySpecialSpawnSound();
                    }
                }
                yield return new WaitForSeconds(stepDelaySeconds);
            }

            boardRenderer.Render(presentation.FinalBoard);
            IsPlaying = false;
        }

        private void AddIfNotNull(List<BoardSnapshot> snapshots, BoardSnapshot snapshot)
        {
            if (snapshot != null)
            {
                snapshots.Add(snapshot);
            }
        }
        
        private static int CountPieces(BoardSnapshot snapshot)
        {
            int count = 0;

            for (int row = 0; row < snapshot.Height; row++)
            {
                for (int col = 0; col < snapshot.Width; col++)
                {
                    if (snapshot.Get(row, col).Kind != PieceKind.Empty)
                        count++;
                }
            }

            return count;
        }
    }
}