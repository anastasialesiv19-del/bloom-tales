using System;
using System.Collections.Generic;
using Core.Randomness;
using JetBrains.Annotations;

namespace Tests.EditMode.TestDoubles
{
    public class SequenceRandomProvider : IRandomProvider
    {
        private readonly int[] _sequence;
        private int _currentIndex = 0;
        public SequenceRandomProvider([CanBeNull] int[] sequence)
        {
            if(sequence == null || sequence.Length == 0) throw new InvalidOperationException("sequence is null or empty");
            _sequence = sequence;
        }

        public int Next(int maxExclusive)
        {
            if(_currentIndex >= _sequence.Length) throw new InvalidOperationException("Sequence overflow");
            var value = _sequence[_currentIndex];
            _currentIndex++;
            return value < 0 || value >= maxExclusive ? throw new InvalidOperationException("Sequence overflow") : value;
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if(_currentIndex >= _sequence.Length) throw new InvalidOperationException("Sequence overflow");
            var value = _sequence[_currentIndex];
            _currentIndex++;
            return value < minInclusive || value >= maxExclusive ? throw new InvalidOperationException("Sequence overflow") : value;
        }
    }
}