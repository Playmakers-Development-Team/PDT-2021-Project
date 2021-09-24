using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

namespace Background.Masking
{
    public class MaskController : MonoBehaviour
    {
        [SerializeField] private float startScale;
        [SerializeField] private float endScale;
        [SerializeField] private float time;

        [SerializeField] private Vector3 aspect = Vector3.one;
        
        [SerializeField] private GameObject maskPrefab;
        
        [SerializeField] private bool spreadOnStart;

        private Transform[] masks;
        
        private void Start()
        {
            masks = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                masks[i] = transform.GetChild(i);
                masks[i].localScale = Vector3.zero;
            }

            if (!spreadOnStart)
                return;
            
            Spread();
        }

        public void Spread() => SpreadAsync(true);
        
        public void Retract() => SpreadAsync(false);

        private async void SpreadAsync(bool appear)
        {
            float total = 0.0f;
            while (total < time)
            {
                if (!Application.isPlaying)
                    return;

                float delta = Mathf.Min(Time.deltaTime, time - total);
                total += delta;

                float scale = Mathf.Lerp(appear ? startScale : endScale,
                    appear ? endScale : startScale, total / time);
                
                SpreadJob job = new SpreadJob
                {
                    scale = scale,
                    aspect = aspect
                };
                TransformAccessArray array = new TransformAccessArray(masks);

                JobHandle handle = job.Schedule(array);
                handle.Complete();
                
                array.Dispose();

                await Task.Yield();
            }
        }

#if UNITY_EDITOR
        public GameObject CreateMask() => (GameObject) PrefabUtility.InstantiatePrefab(maskPrefab, transform);
#endif

        private struct SpreadJob : IJobParallelForTransform
        {
            [ReadOnly] public float scale;
            [ReadOnly] public Vector3 aspect;
            
            public void Execute(int index, TransformAccess transform)
            {
                transform.localScale = aspect * scale;
            }
        }
    }
}
