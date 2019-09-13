using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Plays VATAnimation setting to Renderer Material Property block
    /// </summary>
    public class VATGPUPlayer : MonoBehaviour
    {
        [SerializeField]
        VATAnimation _vatAnimation;
        [SerializeField, Range(0, 1)]
        float _state;
        [SerializeField, Range(0.01f, 5)]
        float _animationSpeed = 1;

        static MaterialPropertyBlock _mpb;


        Renderer _renderer;
        int _positionTexId;
        int _positionTexBId;
        int _rotattionTexId;

        int _boundsCenterId;
        int _boundsExtentsId;
        int _startBoundsCenterId;
        int _startBoundsExtentsId;

        int _stateId;
        int _partsCountId;
        int _highPrecisionMode;
        int _partsIdsInUV3Id;

        bool _recordingPlayed;
        float _startTime;


        public bool IsThereAnimation()
        {
            return _vatAnimation != null;
        }

        public void SetAnimation(VATAnimation vatAnimation)
        {
            _vatAnimation = vatAnimation;
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
            _positionTexBId = Shader.PropertyToID("_PositionsTexB");
            _rotattionTexId = Shader.PropertyToID("_RotationsTex");

            _boundsCenterId = Shader.PropertyToID("_BoundsCenter");
            _boundsExtentsId = Shader.PropertyToID("_BoundsExtents");
            _startBoundsCenterId = Shader.PropertyToID("_StartBoundsCenter");
            _startBoundsExtentsId = Shader.PropertyToID("_StartBoundsExtents");

            _partsCountId = Shader.PropertyToID("_PartsCount");
            _stateId = Shader.PropertyToID("_State");
            _highPrecisionMode = Shader.PropertyToID("_HighPrecisionMode");
            _partsIdsInUV3Id = Shader.PropertyToID("_PartsIdsInUV3");
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
            //TODO set only changed data to MPB
            if (_mpb == null)
            {
                _mpb = new MaterialPropertyBlock();
            }
            _renderer.GetPropertyBlock(_mpb);

            _mpb.SetTexture(_positionTexId, _vatAnimation.PositionsTex);
            _mpb.SetTexture(_rotattionTexId, _vatAnimation.RotationsTex);
            if (_vatAnimation.HighPrecisionPositionMode)
            {
                _mpb.SetTexture(_positionTexBId, _vatAnimation.PositionsTexB);
            }
            _mpb.SetInt(_highPrecisionMode, (_vatAnimation.HighPrecisionPositionMode) ? 1 : 0);
            _mpb.SetInt(_partsIdsInUV3Id, (_vatAnimation.PartsIdsInUV3) ? 1 : 0);

            _mpb.SetFloat(_stateId, _state);
            _mpb.SetInt(_partsCountId, _vatAnimation.PartsCount);
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
                float endTime = _startTime + _vatAnimation.Duration / _animationSpeed;
                _state = Mathf.InverseLerp(_startTime, endTime, Time.time);
                if (Time.time > endTime)
                {
                    _recordingPlayed = false;
                }
            }
        }
    }
}