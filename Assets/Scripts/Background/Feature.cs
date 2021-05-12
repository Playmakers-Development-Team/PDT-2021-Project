using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public abstract class Feature : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool active = true;

        [SerializeField, HideInInspector] private string featureName;
        

        protected virtual Vector3Int Threads => new Vector3Int(8, 8, 1);
        
        public bool IsActive => active;
        public ComputeShader Shader => Settings.BackgroundCompute;
        
        
        public abstract void Execute();

        protected abstract int GetKernelIndex();

        protected ComputeBuffer SetInput<T>(T input) where T : IKernelInput
        {
            ComputeBuffer buffer = new ComputeBuffer(1, input.GetSize());
            buffer.SetData(new[] {input});

            Shader.SetBuffer(GetKernelIndex(), input.GetName(), buffer);

            BackgroundManager.MarkToRelease(buffer);
            
            return buffer;
        }

        protected void SetTexture(string textureName, Texture texture)
        {
            Shader.SetTexture(GetKernelIndex(), textureName, texture);
        }

        protected void Dispatch(int xResolution, int yResolution, int zResolution = 1)
        {
            Shader.Dispatch(GetKernelIndex(), xResolution / Threads.x, yResolution / Threads.y, zResolution / Threads.z);
        }
    }
}
