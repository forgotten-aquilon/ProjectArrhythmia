using System;
using UnityEngine;

public class HPColoredBehaviour : ColoredBehaviour, ISelfDestroyer
{
    private Action _action;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        var action = _action;
        _action = null;
        action?.Invoke();
    }

    public void TakeDamage()
    {
        SetTileAmount(TileAmount - 1);
    }

    public void SetDestroyAction(Action action)
    {
        _action = action;
    }

    public void ClearDestroyAction()
    {
        _action = null;
    }
}
