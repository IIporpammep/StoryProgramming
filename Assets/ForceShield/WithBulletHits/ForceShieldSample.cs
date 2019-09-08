using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    /// <summary>
    /// Adds hits on LMB to ForceShieldController using raycast 
    /// </summary>
    public class ForceShieldSample : MonoBehaviour
    {
        [SerializeField]
        float _radius = 1;
        [SerializeField]
        float _duration = 0.5f;

        ForceShieldController _forceShieldController;
        Camera _camera;
        Collider _collider;


        void Awake()
        {
            _forceShieldController = GetComponent<ForceShieldController>();
            _camera = Camera.main;
            _collider = GetComponent<Collider>();
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (_collider.Raycast(ray, out RaycastHit hitInfo, 100))
                {
                    _forceShieldController.AddHit(hitInfo.point, _duration, _radius);
                }
            }
        }
    }
}