#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    /// <summary>
    /// Changes float material uniform over time using MPB 
    /// </summary>
    public class ChangeMaterialUniformOverTime : MonoBehaviour
    {
        [SerializeField]
        string _uniformName;
        [SerializeField]
        Vector2 _fromTo = new Vector2(0, 1);
        [SerializeField]
        float _duration = 1;
        [SerializeField]
        bool _pingPongMode;

        float _timer;
        static MaterialPropertyBlock _mpb;
        Renderer _renderer;


        void Start()
        {
            _renderer = GetComponent<Renderer>();
            if (_mpb == null)
            {
                _mpb = new MaterialPropertyBlock();
            }
        }

        void Update()
        {
            _timer += Time.deltaTime;
            float value;
            if (_pingPongMode)
            {
                value = Mathf.Lerp(_fromTo.x, _fromTo.y, Mathf.PingPong(_timer / _duration, 1));
            }
            else
            {
                if (_timer >= _duration)
                {
                    return;
                }

                value = Mathf.Lerp(_fromTo.x, _fromTo.y, _timer / _duration);
            }

            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(_uniformName, value);
            _renderer.SetPropertyBlock(_mpb);
        }
    }
}
