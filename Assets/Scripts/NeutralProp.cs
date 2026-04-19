using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NeutralProp : StackedTileBehaviour
{
    [SerializeField]
    private Color _color = Color.white;

    protected override Color GetTileColor(int tileIndex)
    {
        return _color;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
