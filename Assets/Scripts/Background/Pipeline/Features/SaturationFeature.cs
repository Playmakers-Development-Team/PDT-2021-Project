using System;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Modifies the total saturation of a texture.
    /// </summary>
    [Serializable]
    public class SaturationFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        
        [SerializeField, Range(-1f, 1f)] private float amount;
        
        
        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(amount));
            SetTexture("output", input);
            
            Dispatch(input.Width, input.Height);
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
