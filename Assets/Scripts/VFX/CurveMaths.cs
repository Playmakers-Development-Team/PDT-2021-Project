using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
    public static class CurveMaths
    {
        public static Vector3 CatmullRom(IReadOnlyList<Anchor> anchors, float t)
        {
            int startIndex = Mathf.FloorToInt(t * (anchors.Count - 1.0f));
            int endIndex = Mathf.Min(anchors.Count - 1, startIndex + 1);

            float intervalTime = t * (anchors.Count - 1) % 1.0f;

            return CatmullRom(anchors[startIndex], anchors[endIndex], intervalTime);
        }
        
        private static Vector3 CatmullRom(Anchor from, Anchor to, float t)
        {
            float t2 = t * t, t3 = t2 * t;
            return (2f * t3 - 3f * t2 + 1f) * from.position
                   + (t3 - 2f * t2 + t) * from.tangent
                   + (-2f * t3 + 3f * t2) * to.position
                   + (t3 - t * t) * to.tangent;
        }

        public readonly struct Anchor
        {
            public readonly Vector3 position;
            public readonly Vector3 tangent;

            public Anchor(Vector3 position, Vector3 tangent)
            {
                this.position = position;
                this.tangent = tangent;
            }
        }
    }
}
