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
    }
}
