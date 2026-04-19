using System;
using UnityEngine;

public interface ISelfDestroyer
{
    public void SetDestroyAction(Action action);
}
