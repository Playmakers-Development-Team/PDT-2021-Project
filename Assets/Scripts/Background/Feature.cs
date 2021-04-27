using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public abstract class Feature
    {
        public virtual void Execute()
        {
            if (IsReady())
                return;
            
            throw new Exception($"Not all parameters for {GetType().Name} were assigned!");
        }

        protected abstract bool IsReady();

        protected abstract int GetKernelIndex();

        protected ComputeBuffer SetInput<T>(T input) where T : IKernelInput
        {
            ComputeBuffer buffer = new ComputeBuffer(1, input.GetSize());
            buffer.SetData(new[] {input});

            Settings.BackgroundCompute.SetBuffer(GetKernelIndex(), input.GetName(), buffer);

            return buffer;
        }
    }
}
