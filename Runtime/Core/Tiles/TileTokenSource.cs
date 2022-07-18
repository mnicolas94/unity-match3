using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3.Core.Tiles
{
    [CreateAssetMenu(fileName = "TileTokenSource", menuName = "Match3/Tiles/TileTokenSource")]
    public class TileTokenSource : TileBase
    {
        [SerializeField] private Sprite sprite;
        
        public TokenSource GetTokenSource()
        {
            return new TokenSource();
        }
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = sprite;
        }

        public static TileTokenSource CreateFromSprite(Sprite sprite)
        {
            var tile = CreateInstance<TileTokenSource>();
            tile.sprite = sprite;
            return tile;
        }
    }
}