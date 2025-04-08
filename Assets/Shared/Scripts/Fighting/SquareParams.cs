using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fighting
{
    [Serializable]
    public class SquareParams
    {
        public float slope = 1;
        public float power = 1;
        public float start = 0;

        public float Sample(float x)
        {
            return slope * Mathf.Pow(x - 1, power) + start;
        }
    }
}