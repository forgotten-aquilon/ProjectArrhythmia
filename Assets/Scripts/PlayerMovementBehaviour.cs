using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private Camera targetCamera;
    [SerializeField]
    private Collider planeCollider;

    [SerializeField]
    private TargetBehaviour _target;

    [SerializeField]
    private GameObject _projectilePrefab;

    private PlayerBehaviour _playerBehaviour;
    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private Vector2 _moveInput;
    private readonly Dictionary<Collider, Vector3> _collisionNormals = new Dictionary<Collider, Vector3>();
    private readonly List<Collider> _staleCollisionColliders = new List<Collider>();

    private Vector2 lookScreenPosition;
    private void Reset()
    {
        ConfigureRigidbody();
    }

    private void Awake()
    {
        ConfigureRigidbody();
    }

    private void FixedUpdate()
    {
        RemoveStaleCollisionNormals();

        var input = Vector2.ClampMagnitude(_moveInput, 1f);
        var desiredVelocity = new Vector3(input.x, 0f, input.y) * _moveSpeed;
        _rigidbody.linearVelocity = ConstrainVelocityByCollisions(desiredVelocity);
    }

    private void OnDisable()
    {
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }

        _collisionNormals.Clear();
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || _projectilePrefab == null || _target == null || _target.GetShootState() is not {} shootState)
        {
            return;
        }

        var originTransform = transform.parent != null ? transform.parent : transform;
        var projectileDirection = Vector3.ProjectOnPlane(
            _target.transform.position - originTransform.position,
            Vector3.up);

        if (projectileDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        var projectileRotation =
            Quaternion.LookRotation(projectileDirection.normalized, Vector3.up) *
            Quaternion.Euler(90f, 0f, 0f);
        var projectileObject = Instantiate(_projectilePrefab, originTransform.position, projectileRotation);
        var projectile = projectileObject.GetComponent<ProjectileBehaviour>();

        projectile.State = shootState;

        if (projectile != null)
        {
            projectile.Launch(projectileDirection);
        }

        IgnoreProjectileCollision(projectileObject);
    }

    public void OnCapture(InputValue value)
    {
        _target.Capture();
    }

    public void OnJump(InputValue value)
    {
        _target.Switch();
    }

    public void OnLook(InputValue value)
    {
        var pointer = Pointer.current;
        if (pointer != null)
        {
            lookScreenPosition = pointer.position.ReadValue();
        }
        else
        {
            lookScreenPosition += value.Get<Vector2>();

            lookScreenPosition.x = Mathf.Clamp(lookScreenPosition.x, 0f, Screen.width);
            lookScreenPosition.y = Mathf.Clamp(lookScreenPosition.y, 0f, Screen.height);
        }

        targetCamera ??= Camera.main;

        if (targetCamera == null || _target == null)
        {
            return;
        }

        var ray = targetCamera.ScreenPointToRay(lookScreenPosition);

        if (!TryGetLookPoint(ray, out var lookPoint))
        {
            return;
        }

        lookPoint.y = -0.2f;

        _target.transform.position = lookPoint;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    private void ConfigureRigidbody()
    {
        _collider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerBehaviour = GetComponent<PlayerBehaviour>();
        _rigidbody.useGravity = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    private void IgnoreProjectileCollision(GameObject projectile)
    {
        if (_collider == null || projectile == null)
        {
            return;
        }

        var projectileColliders = projectile.GetComponentsInChildren<Collider>();

        foreach (var projectileCollider in projectileColliders)
        {
            if (projectileCollider != null)
            {
                Physics.IgnoreCollision(_collider, projectileCollider, true);
            }
        }
    }

    private bool TryGetLookPoint(Ray ray, out Vector3 lookPoint)
    {
        if (planeCollider != null && planeCollider.Raycast(ray, out var planeHit, Mathf.Infinity))
        {
            lookPoint = planeHit.point;
            return true;
        }

        var planeHeight = _target.transform.position.y;
        var lookPlane = new Plane(Vector3.up, new Vector3(0f, planeHeight, 0f));

        if (lookPlane.Raycast(ray, out var distance))
        {
            lookPoint = ray.GetPoint(distance);
            lookPoint.y = planeHeight;
            return true;
        }

        lookPoint = default;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TryIgnoreKeyCollision(collision))
        {
            return;
        }

        UpdateCollisionNormal(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (TryIgnoreKeyCollision(collision))
        {
            return;
        }

        UpdateCollisionNormal(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        _collisionNormals.Remove(collision.collider);
    }

    private bool TryIgnoreKeyCollision(Collision collision)
    {
        var keyBehaviour = collision.collider.GetComponentInParent<KeyBehaviour>();

        if (keyBehaviour == null)
        {
            return false;
        }

        _collisionNormals.Remove(collision.collider);

        _playerBehaviour.Keys.Add(keyBehaviour.GetHeartState());

        var keyColliders = keyBehaviour.GetComponentsInChildren<Collider>();

        Destroy(keyBehaviour.gameObject);

        return true;
    }

    private Vector3 ConstrainVelocityByCollisions(Vector3 desiredVelocity)
    {
        var adjustedVelocity = desiredVelocity;

        foreach (var collisionNormal in _collisionNormals.Values)
        {
            var velocityIntoSurface = Vector3.Dot(adjustedVelocity, collisionNormal);

            if (velocityIntoSurface < 0f)
            {
                adjustedVelocity -= collisionNormal * velocityIntoSurface;
            }
        }

        return adjustedVelocity;
    }

    private void UpdateCollisionNormal(Collision collision)
    {
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
            _collisionNormals.Remove(collision.collider);
            return;
        }

        _collisionNormals[collision.collider] = accumulatedNormal.normalized;
    }

    private void RemoveStaleCollisionNormals()
    {
        if (_collisionNormals.Count == 0)
        {
            return;
        }

        _staleCollisionColliders.Clear();

        foreach (var collisionPair in _collisionNormals)
        {
            var collisionCollider = collisionPair.Key;

            if (collisionCollider == null || !collisionCollider.enabled || !collisionCollider.gameObject.activeInHierarchy)
            {
                _staleCollisionColliders.Add(collisionCollider);
            }
        }

        for (var i = 0; i < _staleCollisionColliders.Count; i++)
        {
            _collisionNormals.Remove(_staleCollisionColliders[i]);
        }
    }
}
