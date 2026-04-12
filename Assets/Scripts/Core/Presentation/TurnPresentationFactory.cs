using System;
using System.Collections.Generic;
using System.Linq;
using Core.Contracts;
using Core.Turns;
using JetBrains.Annotations;

namespace Core.Presentation
{
    public static class TurnPresentationFactory
    {
        public static TurnPresentationModel Create(TurnResult turnResult)
        {
            if(turnResult == null) throw new ArgumentNullException("turnResult");

            BoardSnapshot swapBoard = null;
            BoardSnapshot directActivationBoard = null;
            BoardSnapshot primaryResolveBoard = null;
            BoardSnapshot gravityBoard = null;
            BoardSnapshot refillBoard = null;
            BoardSnapshot shuffleBoard = null;

            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.SwapApplied, out var phase))
                swapBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException( "SwapBoard is null");
            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.DirectSpecialActivation, out phase))
                directActivationBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException( "DirectActivationBoard is null");
            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.PrimaryMatchResolved, out phase))
                primaryResolveBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException("PrimaryResolveBoard is null");
            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.GravityApplied, out phase))
                gravityBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException("GravityBoard is null");
            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.RefillApplied, out phase))
                refillBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException("RefillBoard is null");
            if(TurnTraceQueries.TryGetPhase(turnResult.Trace, TurnPhaseType.ShuffleApplied, out phase))
                shuffleBoard = phase.BoardAfterPhase ?? throw new InvalidOperationException("ShuffleBoard is null");
            
            var cascadePhases = TurnTraceQueries.GetCascadePhases(turnResult.Trace);
            var cascadeBoards = cascadePhases.Select(phaseTrace => phaseTrace.BoardAfterPhase ?? throw new ArgumentNullException("turnResult", "Cascade is null")).ToList();
            return new TurnPresentationModel(turnResult.InitialBoardSnapshot, turnResult.FinalBoardSnapshot, turnResult.WasApplied,
                turnResult.WasShuffled, turnResult.CreatedMatch, turnResult.TotalClearedCells, swapBoard, directActivationBoard, 
                primaryResolveBoard, gravityBoard, refillBoard, shuffleBoard, cascadeBoards);
        }
    }
}