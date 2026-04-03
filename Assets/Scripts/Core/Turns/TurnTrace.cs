using System;
using System.Collections.Generic;

namespace Core.Turns
{
    public readonly struct TurnTrace
    {
        public IReadOnlyList<TurnPhaseTrace> Phases { get; }

        public TurnTrace(IReadOnlyList<TurnPhaseTrace> phases)
        {
            Phases = phases ?? throw new ArgumentNullException(nameof(phases));
        }
    }
}