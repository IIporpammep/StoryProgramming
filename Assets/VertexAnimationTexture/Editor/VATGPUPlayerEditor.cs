using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace StoryProgramming
{
    [CustomEditor(typeof(VATGPUPlayer)), CanEditMultipleObjects]
    public class VATGPUPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            List<VATGPUPlayer> vatPlayers = targets.Cast<VATGPUPlayer>().ToList();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play Animation"))
                {
                    foreach (var item in vatPlayers)
                    {
                        if (item.IsThereAnimation())
                        {
                            item.PlayRecording();
                        }
                    }
                }
            }

            else
            {
                GUILayout.Label("Enter PlayMode to be able to Play Animation");
            }
        }
    }
}