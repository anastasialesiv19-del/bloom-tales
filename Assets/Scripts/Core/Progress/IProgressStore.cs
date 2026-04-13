namespace Core.Progress
{
    public interface IProgressStore
    {
        PlayerProgress Load();
        void Save(PlayerProgress progress);
    }
}