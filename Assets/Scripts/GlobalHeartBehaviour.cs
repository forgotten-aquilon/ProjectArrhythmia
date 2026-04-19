using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(AudioSource))]
public class GlobalHeartBehaviour : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audio;

    [SerializeField]
    private AudioClip _blue;

    [SerializeField]
    private AudioClip _red;

    [SerializeField]
    private AudioClip _yellow;

    private TimeSpan _timer;
    private TimeSpan _delay;
    private Random _random;

    private Dictionary<HeartState, AudioClip> _dict = new Dictionary<HeartState, AudioClip>();

    public static GlobalHeartBehaviour Instance { get; private set; }

    public HeartState State { get; private set; }
    public event Action<HeartState> StateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _dict.Add(HeartState.Blue, _blue);
        _dict.Add(HeartState.Red, _red);
        _dict.Add(HeartState.Yellow, _yellow);

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

            _audio.clip = _dict[State];
            _audio.Play();

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
