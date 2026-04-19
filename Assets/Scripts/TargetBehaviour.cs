using System;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    public enum CursorState
    {
        Synced,
        Unsynced
    }

    [SerializeField]
    private GameObject _inGameCursor;

    [SerializeField]
    private GameObject _inGameCircle;

    [SerializeField]
    private ColorSO _colorPalette;

    private SpriteRenderer _cursorRenderer;
    private SpriteRenderer _circleRenderer;
    private CursorState _cursorState;

    private HeartState? _capturedState;

    [HideInInspector]
    public Vector3 Position { get; set; }

    public void Awake()
    {
        Cursor.visible = false;
    }

    public void Start()
    {
        _cursorRenderer = _inGameCursor.GetComponent<SpriteRenderer>();
        _circleRenderer = _inGameCircle.GetComponent<SpriteRenderer>();
        ChangeCursor(CursorState.Synced);
        GlobalHeartBehaviour.Instance.StateChanged += InstanceOnStateChanged;
    }

    public void ChangeCursor(CursorState state)
    {
        _cursorState = state;

        if (state == CursorState.Synced)
        {
            _cursorRenderer.enabled = true;
            _circleRenderer.enabled = false;
        }
        else
        {
            _cursorRenderer.enabled = false;
            _circleRenderer.enabled = true;
        }
    }

    public void Switch()
    {
        ChangeCursor(Helper.RotateEnum(_cursorState, 1));
    }

    public void Capture()
    {
        if (_cursorState == CursorState.Unsynced)
        {
            _capturedState = GlobalHeartBehaviour.Instance.State;
            _circleRenderer.color = _colorPalette.GetColor(GlobalHeartBehaviour.Instance.State);
        }
    }

    public HeartState? GetShootState()
    {
        return _cursorState switch
        {
            CursorState.Synced => GlobalHeartBehaviour.Instance.State,
            CursorState.Unsynced => _capturedState,
            _ => throw new NotImplementedException()
        };
    }

    private void InstanceOnStateChanged(HeartState obj)
    {
        _cursorRenderer.color = _colorPalette.GetColor(obj);
    }
}
