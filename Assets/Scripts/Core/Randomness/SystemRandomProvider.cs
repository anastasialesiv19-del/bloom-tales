using System;

namespace Core.Randomness
{
     
    public class SystemRandomProvider : IRandomProvider
    {
        private readonly Random _random = new Random();
        
        public int Next(int maxExclusive)
        {
            return _random.Next(maxExclusive);
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }
    }
}