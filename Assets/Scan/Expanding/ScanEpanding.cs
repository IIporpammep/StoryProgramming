using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class ScanEpanding : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 50)]
        float _maxScale = 5;
        [SerializeField, Range(0.1f, 50)]
        float _scanSpeed = 10;


        void Update()
        {
            transform.localScale = Vector3.one * Mathf.PingPong(Time.time * _scanSpeed, _maxScale);
        }
    }
}
