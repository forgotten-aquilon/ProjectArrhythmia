using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGateBehaviour : ColoredBehaviour
{
    public List<HPColoredBehaviour> Keys = new List<HPColoredBehaviour>();

    private Action _action;

    private bool _isHostile = false;

    protected override void Awake()
    {
        base.Awake();

        foreach (var key in Keys)
        {
            key.SetDestroyAction(TakeDamage);
        }
    }

    public void SetAction(Action action)
    {
        _action = action;
    }

    protected override void OnDestroy()
    {
        if (Application.isPlaying && !ApplicationLifecycle.IsTearingDown)
        {
            _action?.Invoke();
        }

        _action = null;
        base.OnDestroy();
    }

    protected override Color GetTileColor(int tileIndex)
    {
        return _colorPalette.GetColor(State.RotateHeartState(tileIndex));
    }

    private void TakeDamage()
    {
        SetTileAmount(TileAmount - 1);
    }

    public void MakeHostile()
    {
        if (!_isHostile)
        {
            foreach (var key in Keys)
            {
                if (key.GetComponent<SpawnerBehaviour>() is {} spawner)
                {
                    spawner.IsActive = true;
                }
            }
        }

        _isHostile = true;
    }
}
