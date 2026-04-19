using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class StaticTileBehaviour : StackedTileBehaviour
{
    [SerializeField]
    private Color _bottomColor = Color.white;

    [SerializeField]
    private Color _topColor = Color.white;

    protected override Color GetTileColor(int tileIndex)
    {
        if (TileCount == 1)
        {
            return _topColor;
        }

        var lerpFactor = tileIndex / (float)(TileCount - 1);
        return Color.Lerp(_topColor, _bottomColor, lerpFactor);
    }
}
