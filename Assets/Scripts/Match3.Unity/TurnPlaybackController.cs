using System;
using System.Collections;
using System.Collections.Generic;
using Core.Contracts;
using Core.Matching;
using Core.Presentation;
using Match3.Unity.Audio;
using UnityEngine;

namespace Match3.Unity
{
    public class TurnPlaybackController : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private float stepDelaySeconds = 0.4f;
        public bool IsPlaying { get; private set; }

        public void Play(TurnPresentationModel presentation)
        {
            if (presentation == null) throw new ArgumentNullException(nameof(presentation));
            StartCoroutine(PlayRoutine(presentation));
        }

        private IEnumerator PlayRoutine(TurnPresentationModel presentation)
        {
            IsPlaying = true;

            var steps = new List<BoardSnapshot>();

            AddIfNotNull(steps, presentation.SwapBoard);
            AddIfNotNull(steps, presentation.DirectActivationBoard);
            AddIfNotNull(steps, presentation.PrimaryResolveBoard);
            AddIfNotNull(steps, presentation.GravityBoard);
            AddIfNotNull(steps, presentation.RefillBoard);
            foreach (var cascadeBoard in presentation.CascadeBoards)
                AddIfNotNull(steps, cascadeBoard);
            AddIfNotNull(steps, presentation.ShuffleBoard);

            int previousPieceCount = CountPieces(presentation.InitialBoard);

            foreach (var snapshot in steps)
            {
                int currentPieceCount = CountPieces(snapshot);

                boardRenderer.Render(snapshot);

                if (GameAudioController.Instance != null)
                {
                    if (ReferenceEquals(snapshot, presentation.DirectActivationBoard))
                    {
                        GameAudioController.Instance.PlaySpecialActivationSound();
                    }
                    else if (currentPieceCount < previousPieceCount)
                    {
                        GameAudioController.Instance.PlayMatchSound();
                    }
                }

                previousPieceCount = currentPieceCount;

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