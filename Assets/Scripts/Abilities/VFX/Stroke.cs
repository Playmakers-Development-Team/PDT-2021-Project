using UnityEngine;
using Random = UnityEngine.Random;

namespace Abilities.VFX
{
    [RequireComponent(typeof(LineRenderer), typeof(Animator)), ExecuteInEditMode]
    public class Stroke : MonoBehaviour
    {
        [Header("Curve Parameters")]
        
        [SerializeField] private int positionCount = 20;
        [SerializeField, Range(0f, 1f)] private float perpendicularDeviationLimit = 0.5f;
        [SerializeField, Range(0f, 1f)] private float parallelDeviationLimit = 0.3f;

        [Header("Component References")]
        
        [SerializeField] private Transform target;

        private LineRenderer lineRenderer;
        private Animator animator;
        private static readonly int aspectId = Shader.PropertyToID("_Aspect");
        private static readonly int noiseTexId = Shader.PropertyToID("_NoiseTex");
        private static readonly int executeId = Animator.StringToHash("execute");

        
        private void Awake()
        {
            TryGetComponent(out lineRenderer);
            TryGetComponent(out animator);
        }


        public void Execute(Vector3 position)
        {
            target.position = position;
            Generate();
        }

        [ContextMenu("Generate")]
        private void Generate()
        {
            // STEP 0. Clear.
            Clear();
            
            // STEP 1. Construct Anchors.
            CurveMaths.Anchor start = new CurveMaths.Anchor(transform.position, Vector3.zero);
            CurveMaths.Anchor end = new CurveMaths.Anchor(target.position, Vector3.zero);

            float tMin = 0.5f - parallelDeviationLimit / 2.0f;
            float tMax = 0.5f + parallelDeviationLimit / 2.0f;
            
            // Parallel deviation.
            float parallelDeviation = Random.Range(tMin, tMax);
            Vector3 centre = Vector3.Lerp(start.position, end.position, parallelDeviation);
            
            // Perpendicular deviation.
            float perpendicularDeviation = 1.0f - Mathf.Abs(parallelDeviation * 2.0f - 1.0f);
            perpendicularDeviation *= Mathf.Lerp(-perpendicularDeviationLimit,
                perpendicularDeviationLimit, Random.value);
            
            Vector3 perpendicular = Vector3.Cross(transform.forward, end.position - start.position).
                normalized;

            CurveMaths.Anchor middle = new CurveMaths.Anchor(centre + perpendicular * perpendicularDeviation,
                end.position - start.position);

            // STEP 2. Calculate and assign LineRenderer positions.
            float length = 0;
            CurveMaths.Anchor[] anchors = {start, middle, end};
            Vector3[] positions = new Vector3[positionCount];
            for (int i = 0; i < positions.Length; i++)
            {
                float t = i / (positionCount - 1.0f);
                positions[i] = CurveMaths.CatmullRom(anchors, t);

                if (i > 0)
                    length += Vector3.Distance(positions[i - 1], positions[i]);
            }

            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPositions(positions);
            
            // STEP 3. Assign Material and Animator parameters.
            if (Application.isPlaying)
            {
                lineRenderer.material.SetFloat(aspectId, length);
                lineRenderer.material.SetTextureOffset(noiseTexId,
                    new Vector2(Random.value, Random.value));

                animator.SetTrigger(executeId);
            }
            else
            {
                lineRenderer.sharedMaterial.SetFloat(aspectId, length);
                lineRenderer.sharedMaterial.SetTextureOffset(noiseTexId,
                    new Vector2(Random.value, Random.value));
            }
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            lineRenderer.positionCount = 0;
        }
    }
}
