using UnityEngine;

[CreateAssetMenu(fileName = "ColorSO", menuName = "Scriptable Objects/ColorSO")]
public class ColorSO : ScriptableObject
{
    [SerializeField]
    private Color _blue = Color.blue;

    [SerializeField]
    private Color _red = Color.red;

    [SerializeField]
    private Color _yellow = Color.yellow;

    [SerializeField]
    private Color _neutral = Color.white;

    public Color Blue => _blue;
    public Color Red => _red;
    public Color Yellow => _yellow;

    public Color Neutral => _neutral;

    public Color GetColor(HeartState state)
    {
        return state switch
        {
            HeartState.Red => _red,
            HeartState.Yellow => _yellow,
            _ => _blue
        };
    }
}
