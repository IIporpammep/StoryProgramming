using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace StoryProgramming
{
    [CustomEditor(typeof(TextureChannelCombiner))]
    public class TextureChannelCombinerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TextureChannelCombiner textureChannelCombiner = (TextureChannelCombiner)target;
            if (GUILayout.Button("Generate"))
            {
                textureChannelCombiner.Generate();
            }
        }
    }
}