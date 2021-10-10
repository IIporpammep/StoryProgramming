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

        Material _material;
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
            _material = GetComponent<Renderer>().material;
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
            _material.SetTexture(_positionTexId, _vatAnimation.PositionsTex);
            _material.SetTexture(_rotattionTexId, _vatAnimation.RotationsTex);
            if (_vatAnimation.HighPrecisionPositionMode)
            {
                _material.SetTexture(_positionTexBId, _vatAnimation.PositionsTexB);
            }
            _material.SetInt(_highPrecisionMode, (_vatAnimation.HighPrecisionPositionMode) ? 1 : 0);
            _material.SetInt(_partsIdsInUV3Id, (_vatAnimation.PartsIdsInUV3) ? 1 : 0);

            _material.SetFloat(_stateId, _state);
            _material.SetInt(_partsCountId, _vatAnimation.PartsCount);
            _material.SetVector(_boundsCenterId, _vatAnimation.BoundsCenter);
            _material.SetVector(_boundsExtentsId, _vatAnimation.BoundsExtents);
            _material.SetVector(_startBoundsCenterId, _vatAnimation.StartBoundsCenter);
            _material.SetVector(_startBoundsExtentsId, _vatAnimation.StartBoundsExtents);
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