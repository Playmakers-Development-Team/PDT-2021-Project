using System;
using System.Threading.Tasks;
using Background.Pipeline;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Turn.Commands;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Background.Masking
{
    public class BackgroundVFX : MonoBehaviour
    {
        [SerializeField] private BackgroundCamera backgroundCamera;
        
        [Header("Wash")]
        
        [SerializeField] private float startScale;
        [SerializeField] private float endScale;
        [SerializeField] private float time;

        [SerializeField] private Vector3 aspect = Vector3.one;
        
        [SerializeField] private GameObject maskPrefab;
        
        [SerializeField] private bool spreadOnStart;
        [SerializeField] private float washDelay;

        [Header("Line")]
        
        [SerializeField] private Gradient gradient;
        [SerializeField] private float lineDuration;
        [SerializeField] private float lineDelay;

        [Header("Particles")]
        
        [SerializeField] private ParticleSystem system;
        

        private Transform[] masks;

        private void OnEnable()
        {
            CommandManager cmd = ManagerLocator.Get<CommandManager>();
            cmd.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnEncounterWon);
            cmd.ListenCommand((Action<NoRemainingPlayerUnitsCommand>) OnEncounterLost);
        }

        private void OnDisable()
        {
            CommandManager cmd = ManagerLocator.Get<CommandManager>();
            cmd.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnEncounterWon);
            cmd.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnEncounterLost);
        }

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

            OnStart();
        }

        public void Spread() => WashAsync(true);
        
        public void Retract() => WashAsync(false);

        private void OnStart()
        {
            system.Play();
            backgroundCamera.LineMaterial.color = gradient.Evaluate(0);
            
            LineAsync(true, lineDelay);
            WashAsync(true, washDelay);
        }

        private async void LineAsync(bool appear, float delay = 0.0f)
        {
            await UniTask.Delay((int) (delay * 1000.0f));
            
            float total = 0.0f;
            while (total < lineDuration)
            {
                if (!Application.isPlaying)
                    return;

                float delta = Mathf.Min(Time.deltaTime, lineDuration - total);
                total += delta;

                Color colour = gradient.Evaluate(appear ? total / lineDuration : 1.0f - total / lineDuration);
                backgroundCamera.LineMaterial.color = colour;

                await Task.Yield();
            }
        }

        private async void WashAsync(bool appear, float delay = 0.0f)
        {
            await UniTask.Delay((int) (delay * 1000.0f));
            
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

        private void OnEncounterWon(NoRemainingEnemyUnitsCommand cmd) => OnGameFinished();

        private void OnEncounterLost(NoRemainingPlayerUnitsCommand cmd) => OnGameFinished();

        private void OnGameFinished()
        {
            system.Stop();
            LineAsync(false);
            WashAsync(false);
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
