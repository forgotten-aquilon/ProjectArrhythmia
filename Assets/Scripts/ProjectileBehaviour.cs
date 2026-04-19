using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour
{
    private const string DefaultColorPaletteAssetPath = "Assets/ScriptableObject/ColorSO.asset";

    [SerializeField]
    private float _speed = 10f;

    [SerializeField]
    private float _offset = 0.1f;

    [SerializeField]
    private float _lifetimeInSeconds = 4;

    [SerializeField]
    private GameObject _collisionParticlePrefab;

    private TimeSpan _timer = TimeSpan.FromSeconds(0);

    private Rigidbody _rigidbody;
    private Vector3 _direction = Vector3.forward;
    private SpriteRenderer[] _spriteRenderers;
    private ColorSO _colorPalette;


    public HeartState State { get; set; }

    public float Speed
    {
        get => _speed;
        set => _speed = Mathf.Max(0f, value);
    }

    public void Launch(Vector3 direction)
    {
        var planarDirection = Vector3.ProjectOnPlane(direction, Vector3.up);

        _direction = planarDirection.sqrMagnitude > Mathf.Epsilon
            ? planarDirection.normalized
            : Vector3.zero;

        if (_rigidbody != null)
        {
            var launchPosition = _rigidbody.position;
            launchPosition.y += _offset;
            _rigidbody.position = launchPosition;
            _rigidbody.linearVelocity = _direction * _speed;
        }

        ApplyStateColor();
    }

    private void Reset()
    {
        ConfigureRigidbody();
        CacheRenderers();
    }

    private void Awake()
    {
        ConfigureRigidbody();
        CacheRenderers();
    }

    private void FixedUpdate()
    {
        if (_timer < TimeSpan.FromSeconds(_lifetimeInSeconds))
        {
            _timer += TimeSpan.FromSeconds(Time.fixedDeltaTime);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (_rigidbody == null)
        {
            return;
        }

        if (_direction.sqrMagnitude <= Mathf.Epsilon || _speed <= Mathf.Epsilon)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        _rigidbody.linearVelocity = _direction * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var impactPosition = collision.contactCount > 0
            ? collision.GetContact(0).point
            : transform.position;

        SpawnCollisionParticles(impactPosition);
        Damage(collision.gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var impactPosition = other.ClosestPoint(transform.position);

        if ((impactPosition - transform.position).sqrMagnitude <= Mathf.Epsilon)
        {
            impactPosition = transform.position;
        }

        SpawnCollisionParticles(impactPosition);
        Damage(other.gameObject);
        Destroy(gameObject);
    }

    private void Damage(GameObject obj)
    {
        if (obj.GetComponentInParent<HPColoredBehaviour>() is {} behaviour && behaviour.GetShiftedState() == State)
        {
            behaviour.TakeDamage();
        }
    }

    private void SpawnCollisionParticles(Vector3 position)
    {
        if (_collisionParticlePrefab == null)
        {
            return;
        }

        var particlesObject = Instantiate(_collisionParticlePrefab, position, _collisionParticlePrefab.transform.rotation);
        var color = GetCurrentColor();

        foreach (var system in particlesObject.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = system.main;
            main.startColor = color;
            system.Clear(false);
            system.Play(false);
        }
    }

    private void ConfigureRigidbody()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    private void OnValidate()
    {
        _speed = Mathf.Max(0f, _speed);

        if (_rigidbody != null)
        {
            ConfigureRigidbody();
        }
    }

    private void CacheRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void ApplyStateColor()
    {
        _spriteRenderers ??= GetComponentsInChildren<SpriteRenderer>(true);
        _colorPalette ??= LoadColorPalette();

        if (_colorPalette == null || _spriteRenderers == null)
        {
            return;
        }

        var color = _colorPalette.GetColor(State);

        foreach (var spriteRenderer in _spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }

    private Color GetCurrentColor()
    {
        _spriteRenderers ??= GetComponentsInChildren<SpriteRenderer>(true);

        if (_spriteRenderers != null)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer != null)
                {
                    return spriteRenderer.color;
                }
            }
        }

        _colorPalette ??= LoadColorPalette();

        return _colorPalette != null ? _colorPalette.GetColor(State) : Color.white;
    }

    private static ColorSO LoadColorPalette()
    {
        var loadedPalettes = Resources.FindObjectsOfTypeAll<ColorSO>();

        if (loadedPalettes != null && loadedPalettes.Length > 0)
        {
            return loadedPalettes[0];
        }

#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<ColorSO>(DefaultColorPaletteAssetPath);
#else
        return null;
#endif
    }
}
