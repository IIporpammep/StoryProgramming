using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
    [SerializeField]
    float _ensitivityX = 3F;
    [SerializeField]
    float _sensitivityY = 3F;
    [SerializeField]
    float _minX = -3;
    [SerializeField]
    float _maxX = 3;
    [SerializeField]
    float _minY = -3;
    [SerializeField]
    float _maxY = 3;

    Transform _pivotX;
    Transform _pivotY;
    float _rotationX;
    float _rotationY;

    void Awake()
    {
        _pivotX = transform.FindRecursively("PivotX");
        _pivotY = transform.FindRecursively("PivotY");
    }

    void Update()
    {
        _rotationX += Input.GetAxis("Mouse X") * _ensitivityX * Time.deltaTime;
        _rotationY -= Input.GetAxis("Mouse Y") * _sensitivityY * Time.deltaTime;

        _rotationX = Mathf.Clamp(_rotationX, _minX, _maxX);
        _rotationY = Mathf.Clamp(_rotationY, _minY, _maxY);

        _pivotY.localRotation = Quaternion.Euler(_rotationY, 0, 0);
        _pivotX.localRotation = Quaternion.Euler(0, _rotationX, 0);
    }
}
