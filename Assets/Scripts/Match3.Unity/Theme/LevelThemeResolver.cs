using Match3.Unity.Menu;
using UnityEngine;

namespace Match3.Unity.Theme
{
    public static class LevelThemeResolver
    {
        public static LevelTheme Resolve(LevelId levelId, Sprite level1Bg, Sprite level2Bg, Sprite level3Bg)
        {
            switch (levelId)
            {
                case LevelId.Level1:
                    return new LevelTheme
                    {
                        background = level1Bg
                    };

                case LevelId.Level2:
                    return new LevelTheme
                    {
                        background = level2Bg
                    };

                case  LevelId.Level3:
                    return new LevelTheme
                    {
                        background = level3Bg
                    };
                default:
                    return new LevelTheme
                    {
                        background = level1Bg
                    };
            }
        }
    }
}