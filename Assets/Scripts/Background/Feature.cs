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
        
        public bool IsActive => active;
        // TODO: Implement this in all subclasses.
        public ComputeShader Shader => Settings.BackgroundCompute;
        
        
        public abstract void Execute();

        protected abstract int GetKernelIndex();

        protected ComputeBuffer SetInput<T>(T input) where T : IKernelInput
        {
            ComputeBuffer buffer = new ComputeBuffer(1, input.GetSize());
            buffer.SetData(new[] {input});

            Settings.BackgroundCompute.SetBuffer(GetKernelIndex(), input.GetName(), buffer);

            BackgroundManager.MarkToRelease(buffer);
            
            return buffer;
        }
    }
}
