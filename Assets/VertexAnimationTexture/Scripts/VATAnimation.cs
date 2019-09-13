using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoryProgramming
{
    /// <summary>
    /// Holdes all data that used by VAT shader 
    /// </summary>
    public class VATAnimation : ScriptableObject
    {
        public Vector3 BoundsCenter;
        public Vector3 BoundsExtents;
        //Used to decode pivots
        public Vector3 StartBoundsCenter;
        public Vector3 StartBoundsExtents;
        public float Frames;
        public int PartsCount;
        public float Duration;
        public bool HighPrecisionPositionMode;
        public bool PartsIdsInUV3;
        public Texture2D PositionsTex;
        [Tooltip("For high precision mode")]
        public Texture2D PositionsTexB;
        public Texture2D RotationsTex;
    }
}