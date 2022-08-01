using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class Lerp
    {
        public static float Linear(float x)
        {
            return x;
        }

        public static float EaseInSquare(float x)
        {
            return Mathf.Pow(x, 2);
        }

        public static float EaseInCube(float x)
        {
            return Mathf.Pow(x, 3);
        }

        public static float EaseInQuad(float x)
        {
            return Mathf.Pow(x, 4);
        }

        public static float EaseOutSquare(float x)
        {
            return 1 - Mathf.Pow(1 - x, 2);
        }

        public static float EaseOutCube(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }

        public static float EaseOutQuad(float x)
        {
            return 1 - Mathf.Pow(1 - x, 4);
        }
    }
}