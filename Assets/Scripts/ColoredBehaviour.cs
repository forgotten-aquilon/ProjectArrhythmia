using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class ColoredBehaviour : StackedTileBehaviour
{
    [SerializeField]
    protected ColorSO _colorPalette;

    [SerializeField]
    private bool _isStatic = false;

    [SerializeField]
    [Range(0, 2)]
    private int _shift = 0;

    [SerializeField]
    public HeartState State = HeartState.Blue;

    private GlobalHeartBehaviour _heartBehaviour;

    protected virtual void Start()
    {
        if (!_isStatic)
        {
            _heartBehaviour = GlobalHeartBehaviour.Instance;
            State = _heartBehaviour.State;
            _heartBehaviour.StateChanged += OnHeartChanged;
        }

        RefreshColors();
    }

    public HeartState GetShiftedState()
    {
        return State.RotateHeartState(_shift);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_heartBehaviour != null && !_isStatic)
        {
            _heartBehaviour.StateChanged -= OnHeartChanged;
        }
    }

    protected virtual void OnHeartChanged(HeartState state)
    {
        State = state;
        RefreshColors();
    }

    protected override Color GetTileColor(int tileIndex)
    {
        return _colorPalette.GetColor(State.RotateHeartState(_shift));
    }
}
