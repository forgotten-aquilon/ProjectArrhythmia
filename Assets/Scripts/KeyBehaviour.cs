using System;
using UnityEngine;

[RequireComponent(typeof(ColoredBehaviour))]
public class KeyBehaviour : MonoBehaviour, IColoringState
{
    private ColoredBehaviour _coloredBehaviour;

    public void Awake()
    {
        _coloredBehaviour = GetComponent<ColoredBehaviour>();
    }

    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public HeartState GetHeartState()
    {
        return _coloredBehaviour.State;
    }
}
