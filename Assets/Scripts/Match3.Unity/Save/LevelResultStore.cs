using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Match3.Unity.Save
{
    public sealed class LevelResultStore
    {
        private readonly string filePath;

        public LevelResultStore()
        {
            filePath = Path.Combine(Application.persistentDataPath, "level_results.json");
        }
        public string FilePath => filePath;

        public void Append(LevelRunResultDto result)
        {
            var data = LoadAll();
            data.Results.Add(result);

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }

        public LevelResultCollectionDto LoadAll()
        {
            if (!File.Exists(filePath))
                return new LevelResultCollectionDto();

            var json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
                return new LevelResultCollectionDto();

            return JsonUtility.FromJson<LevelResultCollectionDto>(json)
                   ?? new LevelResultCollectionDto();
        }
        public void Clear()
        {
            var empty = new LevelResultCollectionDto();
            var json = JsonUtility.ToJson(empty, true);
            File.WriteAllText(filePath, json);
        }
    }
    

    [System.Serializable]
    public sealed class LevelResultCollectionDto
    {
        public List<LevelRunResultDto> Results = new();
    }
    
}