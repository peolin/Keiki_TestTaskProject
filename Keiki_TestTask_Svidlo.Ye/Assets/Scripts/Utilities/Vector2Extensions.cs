using UnityEngine;

namespace Utilities
{
    public static class Vector2Extensions
    {
        // extend Vector2 to check if a point is on a vector
        public static Vector2 ProjectOnSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            Vector2 ap = p - a;
            
            float e = Vector2.Dot(ap, ab);
            float f = Vector2.Dot(ab, ab);

            if (e <= 0f) return a;
            if (e >= f) return b;
            return a + ab * (e / f);
        }
    }
}