using Core.Progress;

namespace Match3.Unity.Save
{
    public static class ProgressMapper
    {
        public static PlayerProgressDto ToDto(PlayerProgress progress)
        {
            return new PlayerProgressDto()
            {
                totalHintsShown = progress.AdaptiveHints.TotalHintsShown,
                turnsPlayed = progress.TurnsPlayed
            };
        }

        public static PlayerProgress ToDomain(PlayerProgressDto dto)
        {
            if (dto == null) 
                return new PlayerProgress(new AdaptiveHintState(0, 0), 0);
            var adaptive = new AdaptiveHintState(0, dto.totalHintsShown);
            return new PlayerProgress(adaptive, dto.turnsPlayed);
        }
    }
}