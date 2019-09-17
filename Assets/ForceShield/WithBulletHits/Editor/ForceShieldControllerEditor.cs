using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace StoryProgramming
{
    [CustomEditor(typeof(ForceShieldController))]
    public class ForceShieldControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ForceShieldController forceShieldController = (target as ForceShieldController);

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play appearing animation"))
                {
                    forceShieldController.PlayAppearingAnimation();
                }
                if (GUILayout.Button("Play disappearing animation"))
                {
                    forceShieldController.PlayDisappearingAnimation();
                }
            }
            else
            {
                GUILayout.Label("Enter PlayMode to be able to Play Animation");
            }
        }
    }
}