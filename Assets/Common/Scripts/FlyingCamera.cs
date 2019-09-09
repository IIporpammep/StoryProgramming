using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    public class FlyingCamera : MonoBehaviour
    {
        [SerializeField]
        float _ensitivityX = 100;
        [SerializeField]
        float _sensitivityY = 100;
        [SerializeField, Range(0, 100)]
        float _moveSpeed = 30;


        Transform _camera;
        Transform _pivotX;
        Transform _pivotY;
        float _rotationX;
        float _rotationY;
        bool _lockCamera;

        void Awake()
        {
            _camera = transform.FindRecursively("Main Camera");
            _pivotX = transform.FindRecursively("PivotX");
            _pivotY = transform.FindRecursively("PivotY");

            transform.localRotation = Quaternion.identity;
            _rotationX = _pivotX.localRotation.eulerAngles.x;
            _rotationY = _pivotY.localRotation.eulerAngles.y;
        }

        void Update()
        {
            if (!_lockCamera)
            {
                _rotationX -= Input.GetAxis("Mouse Y") * _ensitivityX * Time.deltaTime;
                _rotationY += Input.GetAxis("Mouse X") * _sensitivityY * Time.deltaTime;

                _pivotX.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                _pivotY.localRotation = Quaternion.Euler(0, _rotationY, 0);


                if (Input.GetKey(KeyCode.W))
                {
                    transform.position += _camera.forward * _moveSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.position -= _camera.forward * _moveSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.position -= _camera.right * _moveSpeed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.position += _camera.right * _moveSpeed * Time.deltaTime;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _lockCamera = !_lockCamera;
            }
        }
    }
}