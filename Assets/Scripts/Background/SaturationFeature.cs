using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class SaturationFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        
        [SerializeField, Range(-1f, 1f)] private float amount;
        
        
        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(amount));
            Shader.SetTexture(GetKernelIndex(), "output", input);
            Shader.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8, 1);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.SaturationFeature;
        
        
        private struct Input : IKernelInput
        {
            private float amount;

            
            public Input(float amount)
            {
                this.amount = amount;
            }


            public string GetName() => "saturation_in";

            public int GetSize() => sizeof(float);
        }
    }
}
