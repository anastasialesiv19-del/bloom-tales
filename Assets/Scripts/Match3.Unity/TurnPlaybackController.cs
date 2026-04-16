using System;
using System.Collections;
using System.Collections.Generic;
using Core.Contracts;
using Core.Presentation;
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

            foreach (var snapshot in steps)
            {
                boardRenderer.Render(snapshot);
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
    }
}