#pragma warning disable CS0649 //hide never assigned SerializeField warnings
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Rotates transform
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        Vector3 _rotation;


        void Update()
        {
            transform.Rotate(_rotation * Time.deltaTime);
        }
    }
}