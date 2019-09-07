using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StoryProgramming
{
    [CustomEditor(typeof(VATSample))]
    public class VATSampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VATSample vatSample = (target as VATSample);

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Apply Force"))
                {
                    vatSample.ApplyForce();
                }
                if (GUILayout.Button("Apply Force and Call Action"))
                {
                    vatSample.ApplyForceAndCallAction();
                }
            }
            else
            {
                GUILayout.Label("Enter PlayMode to be able to Apply Force");
            }
        }
    }
}