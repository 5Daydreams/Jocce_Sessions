using UnityEngine;

namespace _Code.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ComponentMultiply(this Vector3 self, Vector3 b )
        {
            self.x *= b.x;
            self.y *= b.y;
            self.z *= b.z;
            return self;
        }

        public static Vector3 ComponentDivision(this Vector3 self, Vector3 b)
        {
            self.x /= b.x;
            self.y /= b.y;
            self.z /= b.z;
            return self;
        }
        
    }
}