using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGateBehaviour : ColoredBehaviour
{
    public List<HPColoredBehaviour> Keys = new List<HPColoredBehaviour>();

    protected override void Awake()
    {
        base.Awake();

        foreach (var key in Keys)
        {
            key.SetDestroyAction(TakeDamage);
        }
    }

    protected override void OnDestroy()
    {
        ClearKeyDestroyActions();
        base.OnDestroy();
    }

    protected override void OnTileAmountDepleted()
    {
        ClearKeyDestroyActions();
    }

    protected override Color GetTileColor(int tileIndex)
    {
        return _colorPalette.GetColor(State.RotateHeartState(tileIndex));
    }

    private void TakeDamage()
    {
        SetTileAmount(TileAmount - 1);
    }

    private void ClearKeyDestroyActions()
    {
        foreach (var key in Keys)
        {
            if (key != null)
            {
                key.ClearDestroyAction();
            }
        }
    }
}
