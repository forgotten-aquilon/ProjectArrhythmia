using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _lookOffset = Vector3.up;

    [SerializeField]
    private float _distance = 10f;

    [SerializeField]
    private float _pitch = 35f;

    [SerializeField]
    private float _yaw = 0f;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        var lookPoint = _target.position + _lookOffset;
        var orbitRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        var offset = orbitRotation * (Vector3.back * _distance);

        transform.position = lookPoint + offset;
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }

    private void OnValidate()
    {
        _distance = Mathf.Max(0.1f, _distance);
    }
}
