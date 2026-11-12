using System;
using System.Collections.Generic;
using Core.Contracts;
using Core.Specials;
using Core.Board;

namespace Core.Turns
{
    public readonly struct TurnPhaseTrace
    {
        public TurnPhaseType Type { get;  }
        public int ClearedCells { get; }
        public int SpawnedSpecials { get; }
        public int CascadeIndex { get; }
        public bool BoardShuffled { get; }
        public BoardSnapshot BoardAfterPhase { get; }
        public IReadOnlyList<SpecialSpawnDecision> SpecialSpawns { get; }
        public IReadOnlyList<SpecialActivationEvent> SpecialActivations { get; }
        public IReadOnlyList<PieceFall> Falls { get; }
        public CascadeWaveAnimation CascadeAnimation { get; }

        public TurnPhaseTrace(TurnPhaseType type, int clearedCells = 0, int spawnedSpecials = 0, int cascadeIndex = 0, 
            bool boardShuffled = false, BoardSnapshot boardAfterPhase = null, IReadOnlyList<SpecialSpawnDecision> specialSpawns = null, 
            IReadOnlyList<SpecialActivationEvent> specialActivations = null, IReadOnlyList<PieceFall> falls = null, CascadeWaveAnimation cascadeAnimation = null) : this()
        {
            if (RequiresBoardSnapshot(type) && boardAfterPhase == null)
            {
                throw new ArgumentException("Mutating phase must has a snapshot.");
            }
            if (clearedCells < 0)
            {
                throw new ArgumentException("Cleared cells cannot have negative values.");
            }
            if (spawnedSpecials < 0)
            {
                throw new ArgumentException("Spawned specials cannot have negative values.");
            }
            if (cascadeIndex < 0)
            {
                throw new ArgumentException("Cascade index cannot have negative values.");
            }
            if (type != TurnPhaseType.CascadeWaveResolved && cascadeIndex > 0)
            {
                throw new ArgumentException("Type that is not CascadeWaveResolved cannot have cascade index.");
            }
            if (type == TurnPhaseType.CascadeWaveResolved && cascadeIndex == 0)
            {
                throw new ArgumentException("Type that is CascadeWaveResolved must have cascade index.");
            }
            
            Type = type;
            ClearedCells = clearedCells;
            SpawnedSpecials = spawnedSpecials;
            CascadeIndex = cascadeIndex;
            BoardShuffled = boardShuffled;
            BoardAfterPhase = boardAfterPhase;

            if (specialSpawns == null)
            {
                SpecialSpawns = Array.Empty<SpecialSpawnDecision>();
            }
            else
            {
                SpecialSpawns = new List<SpecialSpawnDecision>(specialSpawns).AsReadOnly();
            }

            if (specialActivations == null)
            {
                SpecialActivations = Array.Empty<SpecialActivationEvent>();
            }
            else
            {
                SpecialActivations = new List<SpecialActivationEvent>(specialActivations).AsReadOnly();
            }
            
            Falls = falls == null ? Array.Empty<PieceFall>() : new List<PieceFall>(falls).AsReadOnly();
            CascadeAnimation = cascadeAnimation;
        }

        private static bool RequiresBoardSnapshot(TurnPhaseType type)
        {
            switch (type)
            {
                case TurnPhaseType.SwapApplied:
                case TurnPhaseType.DirectSpecialActivation:
                case TurnPhaseType.PrimaryMatchResolved:
                case TurnPhaseType.GravityApplied:
                case TurnPhaseType.RefillApplied:
                case TurnPhaseType.CascadeWaveResolved:
                case TurnPhaseType.ShuffleApplied: return true;
                case TurnPhaseType.InvalidSwap:
                case TurnPhaseType.NoMatchCreated:
                case TurnPhaseType.TurnCompleted:
                default: return false;
            }
        }
    }
}