using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VFX
{
    public class StrokeTester : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int resolution;
        [SerializeField, Range(0f, 1f)] private float midpointRange;
        [SerializeField] private float maxDeviation;
        [SerializeField] private float period;
        [SerializeField] private Animator unitAnimator;

        private float previous;
        private static readonly int attackId = Animator.StringToHash("attack");
        private static readonly int noiseTexId = Shader.PropertyToID("_NoiseTex");
        private static readonly int lengthId = Shader.PropertyToID("_Aspect");

        private void Start()
        {
            previous = -period;
        }

        private void Update()
        {
            if (Time.time - previous < period)
                return;

            previous = Time.time;
            Regenerate();
            
            if (unitAnimator && Application.isPlaying)
                unitAnimator.SetTrigger(attackId);
        }


        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            if (!Camera.main || !lineRenderer || !target)
                return;
            
            Anchor start = new Anchor(Vector3.zero, transform.position);
            Anchor end = new Anchor(Vector3.zero, target.position);

            float deviation = Mathf.Lerp(-maxDeviation, maxDeviation, Random.value);
            Vector3 perpendicular =
                Vector3.Cross(end.position - start.position, Camera.main.transform.forward).
                    normalized * deviation;
            
            float tMin = 0.5f - midpointRange / 2.0f;
            float tMax = 0.5f + midpointRange / 2.0f;

            Anchor middle = new Anchor(end.position - start.position,
                Vector3.Lerp(start.position, end.position, Random.Range(tMin, tMax)) +
                perpendicular);

            Vector3[] positions = new Vector3[resolution];
            
            for (int i = 0; i < resolution; i++)
            {
                float t = i / (resolution - 1.0f);
                positions[i] = CatmullRom(new List<Anchor> {start, middle, end}, t);
            }

            lineRenderer.positionCount = resolution;
            lineRenderer.SetPositions(positions);
            
            float length = 0;
            for (int i = 1; i < lineRenderer.positionCount; i++)
            {
                length += Vector3.Distance(lineRenderer.GetPosition(i),
                    lineRenderer.GetPosition(i - 1));
            }
            length /= lineRenderer.widthMultiplier;

            if (Application.isPlaying)
            {
                lineRenderer.material.SetFloat(lengthId, length);
                lineRenderer.material.SetTextureOffset(noiseTexId, new Vector2(Random.value, Random.value));
            }
            else
            {
                lineRenderer.sharedMaterial.SetFloat(lengthId, length);
                lineRenderer.sharedMaterial.SetTextureOffset(noiseTexId, new Vector2(Random.value, Random.value));
            }
        }
        
        private static Vector3 CatmullRom(IReadOnlyList<Anchor> anchors, float t)
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

        private readonly struct Anchor
        {
            public readonly Vector3 tangent;
            public readonly Vector3 position;

            public Anchor(Vector3 tangent, Vector3 position)
            {
                this.tangent = tangent;
                this.position = position;
            }
        }
    }
}
