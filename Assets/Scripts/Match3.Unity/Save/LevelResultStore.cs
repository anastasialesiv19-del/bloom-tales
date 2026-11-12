using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Match3.Unity.Save
{
    public sealed class LevelResultStore
    {
        private readonly string _filePath;

        public LevelResultStore()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "level_results.json");
        }
        
        public string FilePath => _filePath;

        public void Append(LevelRunResultDto result)
        {
            var data = LoadAll();
            data.Results.Add(result);

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_filePath, json);
        }

        public LevelResultCollectionDto LoadAll()
        {
            if (!File.Exists(_filePath))
            {
                return new LevelResultCollectionDto();
            }

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new LevelResultCollectionDto();
            }

            return JsonUtility.FromJson<LevelResultCollectionDto>(json) ?? new LevelResultCollectionDto();
        }
        
        public void Clear()
        {
            var empty = new LevelResultCollectionDto();
            var json = JsonUtility.ToJson(empty, true);
            File.WriteAllText(_filePath, json);
        }
    }
    

    [System.Serializable]
    public sealed class LevelResultCollectionDto
    {
        public List<LevelRunResultDto> Results = new();
    }
    
}