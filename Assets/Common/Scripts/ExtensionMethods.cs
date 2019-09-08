using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float Remap(this float value,float inMin,float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }
    public static Transform FindRecursively(this Transform target, string name)
    {
        for (int i = 0; i < target.childCount; i++)
        {
            var child = target.GetChild(i);
            if (child.name == name)
            {
                return child;
            }

            Transform childOfChild = child.FindRecursively(name);
            if (childOfChild != null)
            {
                return childOfChild;
            }
        }

        return null;
    }
}
