using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _blue;

    [SerializeField]
    private TextMeshPro _red;

    [SerializeField]
    private TextMeshPro _yellow;

    private Dictionary<HeartState, TextMeshPro> _dict = new Dictionary<HeartState, TextMeshPro>();

    void Start()
    {
        _dict[HeartState.Blue] = _blue;
        _dict[HeartState.Red] = _red;
        _dict[HeartState.Yellow] = _yellow;
        InstanceOnStateChanged(HeartState.Blue);
        GlobalHeartBehaviour.Instance.StateChanged += InstanceOnStateChanged;
    }

    private void InstanceOnStateChanged(HeartState obj)
    {
        foreach (var (state, mesh) in _dict)
        {
            if (state == obj)
            {
                mesh.enabled = true;
            }
            else
            {
                mesh.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
