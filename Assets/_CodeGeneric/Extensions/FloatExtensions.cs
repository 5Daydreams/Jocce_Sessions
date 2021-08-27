using UnityEngine;

namespace _Code.Extensions
{
    public static class FloatExtensions
    {
        public static float Sign(this float self)
        {
            return self / Mathf.Abs(self);
        }
    }
}