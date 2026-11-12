using Match3.Unity.Levels;
using UnityEngine;

namespace Match3.Unity.Theme
{
    public static class LevelThemeResolver
    {
        public static LevelTheme Resolve(LevelId levelId, Sprite level1Bg, Sprite level2Bg, Sprite level3Bg)
        {
            return levelId switch
            {
                LevelId.Level1 => new LevelTheme { background = level1Bg },
                LevelId.Level2 => new LevelTheme { background = level2Bg },
                LevelId.Level3 => new LevelTheme { background = level3Bg },
                _ => new LevelTheme { background = level1Bg }
            };
        }
    }
}