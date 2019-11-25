#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace StoryProgramming
{
    /// <summary>
    /// Records rotation and positions creating _frames in _recordingTime sec.
    /// Add actions that change position of target renderers to _callWhenStartRecording 
    /// </summary>
    public class VATRecorder : MonoBehaviour
    {
        [SerializeField]
        GameObject _target;
        [SerializeField, Range(1, 60)]
        int _framesPerSecond = 60;
        [SerializeField]
        float _recordingTime = 3f;
        [Header(("Generates two textures for position."))]
        [SerializeField]
        bool _highPrecisionPosition;
        [Header(("Use FixedUpdate mode to record rigidbodies physics animation"))]
        [SerializeField]
        Mode _mode;
        [SerializeField]
        UnityEvent _callWhenStartRecording;
        
        enum Mode
        {
            Update, FixedUpdate
        }

        int _frames;
        float _timer;
        int _currrentFrame;
        float _frameTime;
        Bounds _bounds;
        Renderer[] _targetRenderers;
        Rigidbody[] _targetRigidBodies;
        bool _recordingStarted;
        List<Vector3>[] _renderersPositions;
        List<Quaternion>[] _renderersRotations;
        Bounds _startBounds;


        public void StartRecording()
        {
            Debug.LogError("Recording Started");
            _target.transform.position = Vector3.zero;
            _target.gameObject.SetActive(true);
            _bounds = new Bounds();
            _targetRenderers = _target.GetComponentsInChildren<Renderer>();
            _targetRigidBodies = _target.GetComponentsInChildren<Rigidbody>();
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

            _frames = Mathf.CeilToInt(_recordingTime * _framesPerSecond);
            _frameTime = 1f / (float)_framesPerSecond;
            _timer = 0;
            _recordingStarted = true;
            _currrentFrame = -1;

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
                if (_mode == Mode.Update)
                {
                    _renderersPositions[i].Add(_targetRenderers[i].transform.position);
                }
                else
                {
                    if (_targetRenderers[i].gameObject != _targetRigidBodies[i].gameObject)
                    {
                        Debug.LogError(_targetRenderers[i].gameObject.name + " " + _targetRigidBodies[i].gameObject.name);
                    }

                    _renderersPositions[i].Add(_targetRigidBodies[i].position);
                }

            }
        }
        void RecordRotations()
        {
            for (int i = 0; i < _renderersRotations.Length; i++)
            {
                if (_mode == Mode.Update)
                {
                    _renderersRotations[i].Add(_targetRenderers[i].transform.rotation);
                }
                else
                {
                    _renderersRotations[i].Add(_targetRigidBodies[i].rotation);
                }
            }
        }


        void Update()
        {
            if (_mode != Mode.Update || _currrentFrame == _frames || !_recordingStarted)
            {
                return;
            }
            UpdateRecording();
        }

        void FixedUpdate()
        {
            if (_mode != Mode.FixedUpdate || _currrentFrame == _frames || !_recordingStarted)
            {
                return;
            }
            UpdateRecording();
        }

        void UpdateRecording()
        {
            if (_framesPerSecond == 60)
            {
                RecordFrame();
            }
            else
            {
                _timer += Time.deltaTime;
                if (_timer >= _frameTime)
                {
                    _timer -= _frameTime;
                    RecordFrame();
                }
            }

            if (_currrentFrame == _frames)
            {
                RecordingFinished();
            }
        }

        void RecordFrame()
        {
            _currrentFrame++;
            UpdateBounds();
            RecordPositions();
            RecordRotations();
        }

        void RecordingFinished()
        {
            Debug.LogError("Recording Finished");
            VATGenerator vatGenerator = new VATGenerator(_highPrecisionPosition);
            if (_highPrecisionPosition)
            {
                _bounds.Expand(1);//Because EncodeFloatRG won't properly encode on the borders of original bounds.
            }

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