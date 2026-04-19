using System;
using UnityEngine;

public class SpawnerBehaviour : HPColoredBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private HPColoredBehaviour _prefab;

    [SerializeField]
    private DynamicGateBehaviour _parent;

    [SerializeField]
    private int _delayInSecond = 1;

    [SerializeField]
    private float _spawnOffsetX = 1f;

    [SerializeField]
    private float _spawnOffsetZ = 0f;

    [SerializeField]
    private int _hp = 2;

    [SerializeField]
    private int _limit = 10;

    private int _spawned = 0;
    private TimeSpan _timer;

    [SerializeField]
    private bool _isActive = false;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (!_isActive)
            {
                Spawn();
            }
            
            _isActive = value;
        }
    }


    public void FixedUpdate()
    {
        if (!IsActive)
        {
            return;
        }


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

    public override void TakeDamage()
    {
        IsActive = true;
        _parent?.MakeHostile();
        base.TakeDamage();
    }

    private void Spawn()
    {
        if (_prefab == null)
        {
            return;
        }

        if (_spawned >= _limit)
        {
            return;
        }

        Debug.Log("Spwnd");

        var spawnPosition = transform.position;
        spawnPosition.x += _spawnOffsetX;
        spawnPosition.z += _spawnOffsetZ;

        var spawnedObject = Instantiate(_prefab, spawnPosition, transform.rotation);
        spawnedObject.State = GetShiftedState();
        spawnedObject.SetTileAmount(_hp);

        if (_target != null && spawnedObject.TryGetComponent<PursuerBehaviour>(out var pursuer))
        {
            pursuer.Target = _target;
        }

        _spawned++;
    }
}
