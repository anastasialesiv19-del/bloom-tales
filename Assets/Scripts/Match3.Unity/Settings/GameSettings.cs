using System;
using UnityEngine;

namespace Match3.Unity.Settings
{
    [Serializable]
    public class GameSettings
    {
        public float musicVolume = 0.5f;
        public float effectVolume = 0.5f;
        public bool autoHintsEnabled = true;

        public void Normalize()
        {
            musicVolume = Mathf.Clamp01(musicVolume);
            effectVolume = Mathf.Clamp01(effectVolume);
        }
    }
}