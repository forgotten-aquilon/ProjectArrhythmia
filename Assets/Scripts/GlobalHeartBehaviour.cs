using System;
using UnityEngine;
using Random = System.Random;

public class GlobalHeartBehaviour : MonoBehaviour
{
    private TimeSpan _timer;
    private TimeSpan _delay;
    private Random _random;

    public static GlobalHeartBehaviour Instance { get; private set; }

    public HeartState State { get; private set; }
    public event Action<HeartState> StateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _random = new Random();
        _delay = TimeSpan.FromSeconds(_random.Next(2, 5));
        StateChanged?.Invoke(State);
    }

    private void FixedUpdate()
    {
        if (_timer < _delay)
        {
            _timer += TimeSpan.FromSeconds(Time.fixedDeltaTime);
        }
        else
        {
            SetState(State.RotateHeartState(1));
            _timer = TimeSpan.Zero;
            _delay = TimeSpan.FromSeconds(_random.Next(2, 5));
        }
    }

    private void SetState(HeartState state)
    {
        if (State == state)
        {
            return;
        }

        State = state;
        StateChanged?.Invoke(State);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
