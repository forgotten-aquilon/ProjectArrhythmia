using System;
using UnityEngine;

public class Lvl4 : MonoBehaviour
{
    [SerializeField] 
    private DynamicGateBehaviour _startGate;

    [SerializeField]
    private SpawnerBehaviour _a;

    [SerializeField]
    private SpawnerBehaviour _b;

    void Start()
    {
        _a.enabled = false;
        _b.enabled = false;
        _startGate.SetAction(Init);
    }

    private void Init()
    {
        _a.enabled = true;
        _b.enabled = true;
    }
}
