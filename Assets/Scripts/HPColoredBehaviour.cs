using System;
using UnityEngine;

public class HPColoredBehaviour : ColoredBehaviour, ISelfDestroyer
{
    private Action _action;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Application.isPlaying || ApplicationLifecycle.IsTearingDown)
        {
            _action = null;
            return;
        }

        var action = _action;
        _action = null;
        action?.Invoke();
    }

    public virtual void TakeDamage()
    {
        SetTileAmount(TileAmount - 1);
    }

    public void SetDestroyAction(Action action)
    {
        _action = action;
    }
}
