using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3.Core.Tiles
{
    [CreateAssetMenu(fileName = "TileProhibitedPosition", menuName = "Match3/Tiles/TileProhibitedPosition", order = 0)]
    public class TileProhibitedPosition : TileBase
    {
        [SerializeField] private Sprite sprite;
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
        }

        public static TileProhibitedPosition CreateFromSprite(Sprite sprite)
        {
            var tile = CreateInstance<TileProhibitedPosition>();
            tile.sprite = sprite;
            return tile;
        }
    }
}