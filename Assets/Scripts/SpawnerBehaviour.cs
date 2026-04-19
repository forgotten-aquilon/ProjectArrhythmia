using System;
using UnityEngine;

public class SpawnerBehaviour : HPColoredBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private HPColoredBehaviour _prefab;

    [SerializeField]
    private int _delayInSecond = 1;

    [SerializeField]
    private float _spawnOffsetX = 1f;

    [SerializeField]
    private int _hp = 2;

    private TimeSpan _timer;



    public void FixedUpdate()
    {
        if (_timer < TimeSpan.FromSeconds(_delayInSecond))
        {
            _timer += TimeSpan.FromSeconds(Time.fixedDeltaTime);
        }
        else
        {
            Spawn();
            _timer = TimeSpan.Zero;
        }
    }

    private void Spawn()
    {
        if (_prefab == null)
        {
            return;
        }

        var spawnPosition = transform.position;
        spawnPosition.x += _spawnOffsetX;

        var spawnedObject = Instantiate(_prefab, spawnPosition, transform.rotation);
        spawnedObject.State = GetShiftedState();
        spawnedObject.SetTileAmount(_hp);

        if (_target != null && spawnedObject.TryGetComponent<PursuerBehaviour>(out var pursuer))
        {
            pursuer.Target = _target;
        }
    }
}
