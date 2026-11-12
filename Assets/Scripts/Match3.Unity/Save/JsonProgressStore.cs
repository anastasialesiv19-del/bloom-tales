using System.IO;
using Core.Progress;
using UnityEngine;

namespace Match3.Unity.Save
{
    public class JsonProgressStore : IProgressStore
    {
        private readonly string _filePath;
        
        public JsonProgressStore()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "player_progress.json");
        }

        public PlayerProgress Load()
        {
            if (!File.Exists(_filePath))
            {
                return CreateDefaultProgress();
            }
            
            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return CreateDefaultProgress();
            }
            
            var dto = JsonUtility.FromJson<PlayerProgressDto>(json);
            return ProgressMapper.ToDomain(dto);
        }
        
        public void Save(PlayerProgress progress)
        {
            var dto = ProgressMapper.ToDto(progress);
            var json = JsonUtility.ToJson(dto, true);
            File.WriteAllText(_filePath, json);
        }
        
        private static PlayerProgress CreateDefaultProgress()
        {
            return new PlayerProgress(new AdaptiveHintState(0, 0), 0);
        }
    }
}