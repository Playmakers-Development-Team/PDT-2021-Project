using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public abstract class Feature
    {
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
