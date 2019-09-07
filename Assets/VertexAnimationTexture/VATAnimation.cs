using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    public class VATAnimation : ScriptableObject
    {
        public Vector3 BoundsCenter;
        public Vector3 BoundsExtents;
        public Vector3 StartBoundsCenter;
        public Vector3 StartBoundsExtents;
        public float Frames;
        public int RenderersCount;
        public float Duration;
        public Texture2D PositionsTex;
        public Texture2D RotationsTex;
    }
}