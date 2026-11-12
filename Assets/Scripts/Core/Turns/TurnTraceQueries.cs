using System;
using System.Collections.Generic;
using System.Linq;
using Core.Contracts;

namespace Core.Turns
{
    public static  class TurnTraceQueries
    {
        public static bool HasPhase(TurnTrace trace, TurnPhaseType type)
        {
            return trace.Phases.Any(phase => phase.Type == type);
        }

        public static bool TryGetPhase(TurnTrace trace, TurnPhaseType type, out TurnPhaseTrace phase)
        {
            foreach (var currentPhase in trace.Phases)
            {
                if (currentPhase.Type != type)
                {
                    continue;
                }
                phase = currentPhase;
                return true;
            }
            phase = default;
            return false;
        }

        public static IReadOnlyList<TurnPhaseTrace> GetPhases(TurnTrace trace, TurnPhaseType type)
        {
            return trace.Phases.Where(phase => phase.Type == type).ToArray();
        }

        public static IReadOnlyList<TurnPhaseTrace> GetCascadePhases(TurnTrace trace)
        {
             var cascadePhases = trace.Phases.Where(phase => phase.Type == TurnPhaseType.CascadeWaveResolved).ToArray();
             for (int i = 0; i < cascadePhases.Length; i++)
             {
                 if (cascadePhases[i].CascadeIndex != i + 1)
                 {
                     throw  new InvalidOperationException("Cascade phases must be ordered by CascadeIndex starting from 1.");
                 }
             }
             return cascadePhases;
        }

        public static bool TryGetLatestBoardSnapshot(TurnTrace trace, out BoardSnapshot boardSnapshot)
        {
            for (int i = trace.Phases.Count - 1; i >= 0; i--)
            {
                if (trace.Phases[i].BoardAfterPhase != null)
                {
                    boardSnapshot = trace.Phases[i].BoardAfterPhase;
                    return true;
                }
            }
            boardSnapshot = null;
            return false;
        }
        
        public static BoardSnapshot GetLatestBoardSnapshot(TurnTrace trace)
        {
            return TryGetLatestBoardSnapshot(trace, out var boardSnapshot) ? boardSnapshot : throw new InvalidOperationException("No board snapshot found.");
        }
    }
}