using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StoryProgramming
{
    public class EditorPause : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }
        }
    }

}