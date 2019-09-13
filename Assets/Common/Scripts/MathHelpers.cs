using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public static class MathHelpers
    {
        public static float AngleFrom0To360(float angleDeg)
        {
            while (angleDeg >= 360)
            {
                angleDeg -= 360;
            }
            while (angleDeg < 0)
            {
                angleDeg += 360;
            }
            return angleDeg;
        }

        /// <summary>
        /// From UnityCG.cginc
        /// Encoding [0..1) float into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
        /// </summary>
        public static Vector2 EncodeFloatRG(float v)
        {
            Vector2 kEncodeMul = new Vector2(1.0f, 255.0f);
            float kEncodeBit = 1.0f / 255.0f;
            Vector2 enc = kEncodeMul * v;
            enc = new Vector2(enc.x - Mathf.Floor(enc.x), enc.y - Mathf.Floor(enc.y));
            enc.x -= enc.y * kEncodeBit;
            return enc;
        }

        /// <summary>
        /// From UnityCG.cginc
        /// Decodes [0..1) float into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
        /// </summary>
        public static float DecodeFloatRG(Vector2 enc)
        {
            Vector2 kDecodeDot = new Vector2(1.0f, 1.0f / 255.0f);
            return Vector2.Dot(enc, kDecodeDot);
        }
    }
}
