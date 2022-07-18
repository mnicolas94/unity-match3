using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3.Core.Tiles
{
    [CreateAssetMenu(fileName = "TileToken", menuName = "Match3/Tiles/TileToken")]
    public class TileToken : TileBase
    {
        [SerializeField] private TokenData tokenData;

        public TokenData TokenData => tokenData;
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = tokenData.TokenSprite;
        }

        public static TileToken CreateFromTokenData(TokenData tokenData)
        {
            var tile = CreateInstance<TileToken>();
            tile.tokenData = tokenData;
            return tile;
        }
    }
}