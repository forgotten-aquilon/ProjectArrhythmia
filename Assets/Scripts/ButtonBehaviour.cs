using UnityEngine;

[RequireComponent(typeof(ColoredBehaviour))]
public class ButtonBehaviour : MonoBehaviour, IColoringState
{
    private ColoredBehaviour _coloredBehaviour;
    private GameObject _gate;

    public void Awake()
    {
        _coloredBehaviour = GetComponent<ColoredBehaviour>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public HeartState GetHeartState()
    {
        return _coloredBehaviour.State;
    }

    public void Trigger()
    {
        var gate = _gate.GetComponent<StaticGateBehaviour>();

        
    }
}
