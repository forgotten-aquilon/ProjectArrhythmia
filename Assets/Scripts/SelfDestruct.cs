using System;
using Unity.Mathematics;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private int _delay = 5;

    private TimeSpan _timer;
    void Start()
    {
        
    }

    public void FixedUpdate()
    {
        if (_timer < TimeSpan.FromSeconds(_delay))
        {
            _timer += TimeSpan.FromSeconds(Time.fixedDeltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
