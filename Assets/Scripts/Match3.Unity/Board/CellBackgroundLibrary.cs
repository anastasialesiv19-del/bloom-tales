using UnityEngine;

namespace Match3.Unity.Board
{
    public class CellBackgroundLibrary : MonoBehaviour
    {
        [SerializeField] private Sprite defaultTileSprite;
        [SerializeField] private Sprite variantTileSprite;

        public Sprite GetDefault()
        {
          return defaultTileSprite;  
        }

        public Sprite GetVariant()
        {
            return variantTileSprite;
        }

        public Sprite GetFor(bool useVariant)
        {
            return useVariant ? variantTileSprite : defaultTileSprite;
        }
    }
}