using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace StoryProgramming
{
    [CustomEditor(typeof(VATPlayer))]
    public class VATPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VATPlayer vatPlayer = (target as VATPlayer);

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