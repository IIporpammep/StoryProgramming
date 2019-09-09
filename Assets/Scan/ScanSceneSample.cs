using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class ScanSceneSample : MonoBehaviour
    {
        [SerializeField]
        GameObject _expandingScanPrefab;
        [SerializeField]
        GameObject _scanFromCameraPrefab;
        [SerializeField]
        GameObject _scanFromCameraWithTexturePrefab;

        Camera _camera;


        void Awake()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100))
                {
                    GameObject.Instantiate(_expandingScanPrefab, hitInfo.point, Quaternion.identity);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                GameObject.Instantiate(_scanFromCameraPrefab);

            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Don't create the version with texture until it finished
                //   GameObject.Instantiate(_scanFromCameraWithTexturePrefab);
            }
        }
    }
}
