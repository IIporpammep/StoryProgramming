using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace StoryProgramming
{
    [CustomEditor(typeof(VATGPUPlayer))]
    public class VATGPUPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VATGPUPlayer vatPlayer = (target as VATGPUPlayer);

            if (Application.isPlaying)
            {
                if (vatPlayer.IsThereAnimation())
                {
                    if (GUILayout.Button("Play Animation"))
                    {
                        vatPlayer.PlayRecording();
                    }
                }
                else
                {
                    GUILayout.Label("Generate Animation with VATRecorder and set the generated animation to _vatAnimation");
                }
            }
            else
            {
                GUILayout.Label("Enter PlayMode to be able to Play Animation");
            }
        }
    }
}