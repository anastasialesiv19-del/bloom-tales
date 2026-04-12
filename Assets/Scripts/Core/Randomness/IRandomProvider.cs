namespace Core.Randomness
{
    public interface IRandomProvider
    {
        public int Next(int maxExclusive);
        public int Next(int minInclusive, int maxExclusive);
    }
}