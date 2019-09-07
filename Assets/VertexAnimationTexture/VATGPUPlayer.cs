using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    public class VATGPUPlayer : MonoBehaviour
    {
        [SerializeField]
        VATAnimation _vatAnimation;
        [SerializeField]
        [Range(0, 1)]
        float _state;

        static MaterialPropertyBlock _mpb;


        Renderer _renderer;
        int _positionTexId;
        int _rotattionTexId;
        int _framesId;

        int _boundsCenterId;
        int _boundsExtentsId;
        int _startBoundsCenterId;
        int _startBoundsExtentsId;

        int _stateId;

        bool _recordingPlayed;
        float _startTime;


        public bool IsThereAnimation()
        {
            return _vatAnimation != null;
        }

        public void PlayRecording()
        {
            _recordingPlayed = true;
            _startTime = Time.time;
        }

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _positionTexId = Shader.PropertyToID("_PositionsTex");
            _rotattionTexId = Shader.PropertyToID("_RotationsTex");
            _framesId = Shader.PropertyToID("_Frames");

            _boundsCenterId = Shader.PropertyToID("_BoundsCenter");
            _boundsExtentsId = Shader.PropertyToID("_BoundsExtents");
            _startBoundsCenterId = Shader.PropertyToID("_StartBoundsCenter");
            _startBoundsExtentsId = Shader.PropertyToID("_StartBoundsExtents");

            _stateId = Shader.PropertyToID("_State");
        }

        void Update()
        {
            if (!IsThereAnimation())
            {
                return;
            }

            UpdateAnimation();
            SendDataToRenderer();
        }

        void SendDataToRenderer()
        {
            if (_mpb == null)
            {
                _mpb = new MaterialPropertyBlock();
            }
            _renderer.GetPropertyBlock(_mpb);

            _mpb.SetTexture(_positionTexId, _vatAnimation.PositionsTex);
            _mpb.SetTexture(_rotattionTexId, _vatAnimation.RotationsTex);

            _mpb.SetFloat(_stateId, _state);
            _mpb.SetFloat(_framesId, _vatAnimation.Frames);

            _mpb.SetVector(_boundsCenterId, _vatAnimation.BoundsCenter);
            _mpb.SetVector(_boundsExtentsId, _vatAnimation.BoundsExtents);
            _mpb.SetVector(_startBoundsCenterId, _vatAnimation.StartBoundsCenter);
            _mpb.SetVector(_startBoundsExtentsId, _vatAnimation.StartBoundsExtents);

            _renderer.SetPropertyBlock(_mpb);
        }

        void UpdateAnimation()
        {
            if (_recordingPlayed)
            {
                _state = Mathf.InverseLerp(_startTime, _startTime + _vatAnimation.Duration, Time.time);
                if (Time.time > _startTime + _vatAnimation.Duration)
                {
                    _recordingPlayed = false;
                }
            }
        }
    }
}