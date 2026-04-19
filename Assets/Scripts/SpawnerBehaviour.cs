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

    [SerializeField]
    private int _limit = 10;

    private int _spawned = 0;
    private TimeSpan _timer;



    public void FixedUpdate()
    {
        if (_timer < TimeSpan.FromSeconds(_delayInSecond))
        {
            _timer += TimeSpan.FromSeconds(Time.fixedDeltaTime);
        }
        else
        {
            if (_spawned < _limit)
            {
                Spawn();
            }

            _timer = TimeSpan.Zero;
            _spawned++;
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
