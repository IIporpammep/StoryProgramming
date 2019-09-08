using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class ScanFromCamera : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;
        [SerializeField, Range(0.1f, 100)]
        float _maxDistance = 5;
        [SerializeField, Range(0.1f, 50)]
        float _scanSpeed = 10;


        void Update()
        {
            float distance = Mathf.PingPong(Time.time * _scanSpeed, _maxDistance);
            transform.position = _camera.transform.position + _camera.transform.forward * distance;
            transform.forward = _camera.transform.forward;

            float frustumHeight = 2.0f * distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * _camera.aspect;
            transform.localScale = new Vector3(frustumWidth, frustumHeight, 1);
        }
    }
}
