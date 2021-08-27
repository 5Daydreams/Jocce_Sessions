using System;
using UnityEngine;

namespace _Code.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Unlike the '*' operator, this returns a Vector3
        /// </summary>
        /// <param name="self"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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
        
        public static Vector3 ComponentModulo(this Vector3 self, Vector3 b)
        {
            self.x %= b.x;
            self.y %= b.y;
            self.z %= b.z;
            return self;
        }
        
        public static Vector2 ComponentMultiply(this Vector2 self, Vector2 b )
        {
            self.x *= b.x;
            self.y *= b.y;
            return self;
        }
        
        public static Vector3Int ConvertToVector3Int(this Vector3 self)
        {
            int x = Convert.ToInt32(self.x);
            int y = Convert.ToInt32(self.y);
            int z = Convert.ToInt32(self.z);
            return new Vector3Int(x,y,z);
        }
        
    }
}