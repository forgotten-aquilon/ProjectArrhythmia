using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PursuerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _collisionPushbackSpeed = 1f;

    [SerializeField]
    private float _collisionPushbackDamping = 12f;

    private Rigidbody _rigidbody;
    private Vector3 _pushbackVelocity;

    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = Mathf.Max(0f, value);
    }

    private void Reset()
    {
        ConfigureRigidbody();
    }

    private void Awake()
    {
        ConfigureRigidbody();
    }

    public void OnDestroy()
    {
        
    }

    private void FixedUpdate()
    {
        DecayPushback();

        if (_target == null)
        {
            _rigidbody.linearVelocity = _pushbackVelocity;
            _rigidbody.angularVelocity = Vector3.zero;
            return;
        }

        var offset = _target.position - transform.position;
        var planarOffset = Vector3.ProjectOnPlane(offset, Vector3.up);

        if (planarOffset.sqrMagnitude <= Mathf.Epsilon)
        {
            _rigidbody.linearVelocity = _pushbackVelocity;
            _rigidbody.angularVelocity = Vector3.zero;
            return;
        }

        _rigidbody.linearVelocity = planarOffset.normalized * _speed + _pushbackVelocity;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        _pushbackVelocity = Vector3.zero;
    }

    private void OnValidate()
    {
        _speed = Mathf.Max(0f, _speed);
        _collisionPushbackSpeed = Mathf.Max(0f, _collisionPushbackSpeed);
        _collisionPushbackDamping = Mathf.Max(0f, _collisionPushbackDamping);

        if (_rigidbody != null)
        {
            ConfigureRigidbody();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyCollisionPushback(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        ApplyCollisionPushback(collision);
    }

    private void ConfigureRigidbody()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void ApplyCollisionPushback(Collision collision)
    {
        if (_collisionPushbackSpeed <= Mathf.Epsilon)
        {
            return;
        }

        var accumulatedNormal = Vector3.zero;

        for (var i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);
            var horizontalNormal = Vector3.ProjectOnPlane(contact.normal, Vector3.up);

            if (horizontalNormal.sqrMagnitude > Mathf.Epsilon)
            {
                accumulatedNormal += horizontalNormal.normalized;
            }
        }

        if (accumulatedNormal.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        _pushbackVelocity = accumulatedNormal.normalized * _collisionPushbackSpeed;
    }

    private void DecayPushback()
    {
        if (_pushbackVelocity.sqrMagnitude <= Mathf.Epsilon)
        {
            _pushbackVelocity = Vector3.zero;
            return;
        }

        _pushbackVelocity = Vector3.MoveTowards(
            _pushbackVelocity,
            Vector3.zero,
            _collisionPushbackDamping * Time.fixedDeltaTime);
    }
}
