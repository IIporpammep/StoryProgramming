#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Used to test VAT encoding/decoding using CPU, VATGenerator.SetTextureSettings can set isReadable if uncomment
    /// </summary>
    public class VATPlayer : MonoBehaviour
    {
        [SerializeField]
        GameObject _target;
        [SerializeField]
        VATAnimation _vatAnimation;

        float _timer;
        int _currrentFrame;
        float _frameTime;
        Renderer[] _targetRenderers;
        bool _recordingPlayed;

        public bool IsThereAnimation()
        {
            return _vatAnimation != null;
        }

        public void PlayRecording()
        {
            _targetRenderers = _target.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < _targetRenderers.Length; i++)
            {
                GameObject.Destroy(_targetRenderers[i].gameObject.GetComponent<Rigidbody>());
                GameObject.Destroy(_targetRenderers[i].gameObject.GetComponent<Collider>());
            }


            _frameTime = _vatAnimation.Duration / (float)_vatAnimation.Frames;
            _recordingPlayed = true;
        }

        void Update()
        {
            if (_vatAnimation == null || _currrentFrame == _vatAnimation.Frames || !_recordingPlayed)
            {
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= _frameTime)
            {
                _timer -= _frameTime;
                _currrentFrame++;
                GetPositions();
                GetRotation();
            }
        }

        void GetPositions()
        {
            for (int x = 0; x < _targetRenderers.Length; x++)
            {
                Color color = _vatAnimation.PositionsTex.GetPixel(x, _currrentFrame);
                _targetRenderers[x].transform.position = _vatAnimation.BoundsCenter + new Vector3(Mathf.Lerp(-_vatAnimation.BoundsExtents.x, _vatAnimation.BoundsExtents.x, color.r),
                                               Mathf.Lerp(-_vatAnimation.BoundsExtents.y, _vatAnimation.BoundsExtents.y, color.g),
                                               Mathf.Lerp(-_vatAnimation.BoundsExtents.z, _vatAnimation.BoundsExtents.z, color.b));

            }
        }
        void GetRotation()
        {
            for (int x = 0; x < _targetRenderers.Length; x++)
            {
                Color color = _vatAnimation.RotationsTex.GetPixel(x, _currrentFrame);
                _targetRenderers[x].transform.rotation = new Quaternion(color.r.Remap(0, 1, -1, 1), color.g.Remap(0, 1, -1, 1), color.b.Remap(0, 1, -1, 1), color.a.Remap(0, 1, -1, 1));
                //_targetRenderers[x].transform.rotation = Quaternion.Euler(color.r * 360, color.g * 360, color.b * 360);
            }
        }
    }
}