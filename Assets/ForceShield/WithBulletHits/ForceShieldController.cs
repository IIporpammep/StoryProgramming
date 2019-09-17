using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Stores hits and sends them to renderer
    /// </summary>
    public class ForceShieldController : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        float _DissolveValue;
        [SerializeField, Range(0, 10)]
        float _animationDuration = 2;

        const int MAX_HITS_COUNT = 10;

        Renderer _renderer;
        MaterialPropertyBlock _mpb;

        int _hitsCount;
        Vector4[] _hitsObjectPosition = new Vector4[MAX_HITS_COUNT];
        float[] _hitsDuration = new float[MAX_HITS_COUNT];
        float[] _hitsTimer = new float[MAX_HITS_COUNT];
        float[] _hitRadius = new float[MAX_HITS_COUNT];

        //1(max)..0(end of life time)
        float[] _hitsIntensity = new float[MAX_HITS_COUNT];

        bool _recordingPlayed;
        float _startTime;
        float _finishTime;
        float _targetDissolveValue;


        public void AddHit(Vector3 worldPosition, float duration, float radius)
        {
            int id = GetFreeHitId();
            _hitsObjectPosition[id] = transform.InverseTransformPoint(worldPosition);
            _hitsDuration[id] = duration;
            _hitRadius[id] = radius;

            _hitsTimer[id] = 0;
        }
        int GetFreeHitId()
        {
            if (_hitsCount < MAX_HITS_COUNT)
            {
                _hitsCount++;
                return _hitsCount - 1;
            }
            else
            {
                float minDuration = float.MaxValue;
                int minId = 0;
                for (int i = 0; i < MAX_HITS_COUNT; i++)
                {
                    if (_hitsDuration[i] < minDuration)
                    {
                        minDuration = _hitsDuration[i];
                        minId = i;
                    }
                }
                return minId;
            }
        }

        public void ClearAllHits()
        {
            _hitsCount = 0;
            SendHitsToRenderer();
        }

        public void PlayAppearingAnimation()
        {
            _recordingPlayed = true;
            _finishTime = Time.time + Mathf.Lerp(0, _animationDuration, _DissolveValue);
            _targetDissolveValue = 0;
        }

        public void PlayDisappearingAnimation()
        {
            _recordingPlayed = true;
            _finishTime = Time.time + Mathf.Lerp(0, _animationDuration, 1 - _DissolveValue);
            _targetDissolveValue = 1;
        }


        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _mpb = new MaterialPropertyBlock();
        }

        void Update()
        {
            UpdateAnimation();
            UpdateHitsLifeTime();
            SendHitsToRenderer();
        }
        void UpdateHitsLifeTime()
        {
            for (int i = 0; i < _hitsCount;)
            {
                _hitsTimer[i] += Time.deltaTime;
                if (_hitsTimer[i] > _hitsDuration[i])
                {
                    SwapWithLast(i);
                }
                else
                {
                    i++;
                }
            }
        }
        void SwapWithLast(int id)
        {
            int idLast = _hitsCount - 1;
            if (id != idLast)
            {
                _hitsObjectPosition[id] = _hitsObjectPosition[idLast];
                _hitsDuration[id] = _hitsDuration[idLast];
                _hitsTimer[id] = _hitsTimer[idLast];
                _hitRadius[id] = _hitRadius[idLast];
            }
            _hitsCount--;
        }
        void UpdateAnimation()
        {
            if (_recordingPlayed)
            {
                _DissolveValue = Mathf.Lerp(1 - _targetDissolveValue, _targetDissolveValue, Mathf.InverseLerp(_finishTime - _animationDuration, _finishTime, Time.time));
                if (Time.time > _finishTime)
                {
                    _recordingPlayed = false;
                }
            }
        }


        void SendHitsToRenderer()
        {
            _renderer.GetPropertyBlock(_mpb);

            _mpb.SetFloat("_DissolveValue", _DissolveValue);
            _mpb.SetFloat("_HitsCount", _hitsCount);
            _mpb.SetFloatArray("_HitsRadius", _hitRadius);

            for (int i = 0; i < _hitsCount; i++)
            {
                if (_hitsDuration[i] > 0f)
                {
                    _hitsIntensity[i] = 1 - Mathf.Clamp01(_hitsTimer[i] / _hitsDuration[i]);
                }
            }

            _mpb.SetVectorArray("_HitsObjectPosition", _hitsObjectPosition);
            _mpb.SetFloatArray("_HitsIntensity", _hitsIntensity);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}