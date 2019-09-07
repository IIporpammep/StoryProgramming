using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace StoryProgramming
{
    public class VATRecorder : MonoBehaviour
    {
        [SerializeField]
        GameObject _target;
        [SerializeField]
        int _frames = 128;
        [SerializeField]
        float _recordingTime = 3f;
        [SerializeField]
        bool _highPrecisionPosition;
        [SerializeField]
        UnityEvent _callWhenStartRecording;

        float _timer;
        int _currrentFrame;
        float _frameTime;
        Bounds _bounds;
        Renderer[] _targetRenderers;
        bool _recordingStarted;
        List<Vector3>[] _renderersPositions;
        List<Quaternion>[] _renderersRotations;
        Bounds _startBounds;


        public void StartRecording()
        {
            _bounds = new Bounds();
            _targetRenderers = _target.GetComponentsInChildren<Renderer>();
            _renderersPositions = new List<Vector3>[_targetRenderers.Length];
            for (int i = 0; i < _renderersPositions.Length; i++)
            {
                _renderersPositions[i] = new List<Vector3>();
            }
            _renderersRotations = new List<Quaternion>[_targetRenderers.Length];
            for (int i = 0; i < _renderersRotations.Length; i++)
            {
                _renderersRotations[i] = new List<Quaternion>();
            }

            _frameTime = _recordingTime / (float)_frames;
            _timer = 0;
            _recordingStarted = true;
            _currrentFrame = 1;

            VATMeshGenerator vatMeshGenerator = new VATMeshGenerator();
            UpdateBounds();
            _startBounds = _bounds;
            vatMeshGenerator.GenerateMesh(_target.name, _targetRenderers, _startBounds);

            RecordFrame();

            if (_callWhenStartRecording != null)
            {
                _callWhenStartRecording.Invoke();
            }
        }

        void UpdateBounds()
        {
            for (int i = 0; i < _targetRenderers.Length; i++)
            {
                _bounds.Encapsulate(_targetRenderers[i].bounds);
            }
        }
        void RecordPositions()
        {
            for (int i = 0; i < _targetRenderers.Length; i++)
            {
                _renderersPositions[i].Add(_targetRenderers[i].transform.position);
            }
        }
        void RecordRotations()
        {
            for (int i = 0; i < _renderersRotations.Length; i++)
            {
                _renderersRotations[i].Add(_targetRenderers[i].transform.rotation);
            }
        }


        void Update()
        {
            if (_currrentFrame == _frames || !_recordingStarted)
            {
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= _frameTime)
            {
                _timer -= _frameTime;
                _currrentFrame++;

                RecordFrame();
            }
            if (_currrentFrame == _frames)
            {
                RecordingFinished();
            }
        }

        void RecordFrame()
        {
            UpdateBounds();
            RecordPositions();
            RecordRotations();
        }

        void RecordingFinished()
        {
            Debug.LogError("Recording Finished");
            VATGenerator vatGenerator = new VATGenerator(_highPrecisionPosition);
            vatGenerator.GenerateVAT(_target.name, _targetRenderers.Length, _recordingTime, _frames, _bounds, _startBounds, _renderersPositions, _renderersRotations);
        }
        
        void OnDrawGizmos()
        {
            if (_currrentFrame == _frames)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }
    }
}