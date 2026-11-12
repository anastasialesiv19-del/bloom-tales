using Match3.Unity.GameMode;

namespace Match3.Unity.GameFlow
{
    public static class GameResultResolver
    {
        public static GameResultType Resolve(GameModeType mode, bool isExpired)
        {
            return !isExpired ? GameResultType.None : GameResultType.Defeat;
        }
    }
}