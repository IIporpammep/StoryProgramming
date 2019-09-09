using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class ScanFromCamera : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 100)]
        float _maxDistance = 5;
        [SerializeField, Range(0.1f, 20)]
        float _lifeTime = 10;


        float _startTime;
        Camera _camera;


        void Awake()
        {
            _camera = Camera.main;
            _startTime = Time.time;
            UpdatePositionAndScale();
        }

        void Update()
        {
            UpdatePositionAndScale();

            if (Time.time >= _startTime + _lifeTime)
            {
                Destroy(gameObject);
            }
        }

        private void UpdatePositionAndScale()
        {
            float distance = _maxDistance * Mathf.InverseLerp(_startTime, _startTime + _lifeTime, Time.time);
            transform.position = _camera.transform.position + _camera.transform.forward * distance;
            transform.forward = _camera.transform.forward;

            float frustumHeight = 2.0f * distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * _camera.aspect;
            transform.localScale = new Vector3(frustumWidth, frustumHeight, 1);
        }
    }
}
