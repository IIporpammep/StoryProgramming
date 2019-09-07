using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StoryProgramming
{
    [CustomEditor(typeof(VATRecorder))]
    public class VATRecorderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VATRecorder vatRecorder = (target as VATRecorder);

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Start Recording"))
                {
                    vatRecorder.StartRecording();
                }
            }
            else
            {
                GUILayout.Label("Enter PlayMode to be able to Start Recording");
            }
        }
    }
}