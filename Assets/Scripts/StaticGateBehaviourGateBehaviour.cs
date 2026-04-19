using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class StaticGateBehaviour : StaticTileBehaviour
{
    [SerializeField]
    private ColorSO _colorPalette;
    
    private HashSet<HeartState> _colors = new HashSet<HeartState>();

    public List<HPColoredBehaviour> Keys = new List<HPColoredBehaviour>();


    void Start()
    {
        _colors.Add(HeartState.Blue);
        _colors.Add(HeartState.Red);
        _colors.Add(HeartState.Yellow);

        RefreshColors();

        foreach (var key in Keys)
        {
            key.SetDestroyAction(() => TakeDamage(key.State));
        }
    }


    public void TakeDamage(HeartState state)
    {
        _colors.Remove(state);
        SetTileAmount(TileAmount - 1);
    }

    protected override Color GetTileColor(int tileIndex)
    {
        int counter = 0;
        foreach (var color in _colors)
        {
            if (counter == tileIndex)
            {
                return _colorPalette.GetColor(color);
            }
            counter++;
        }

        return _colorPalette.Neutral;
    }
}
