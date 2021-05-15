using System;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Varies the opacity of a texture, according to a strength map.
    /// </summary>
    [Serializable]
    public class OpacityShiftFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private TextureParameters strengthMap;

        [SerializeField, Range(0f, 1f)] private float amount;
        [SerializeField, Range(-1f, 1f)] private float balance;
        
        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(strengthMap.Parameters, amount, balance));
            SetTexture("_tex1", strengthMap.Texture);
            SetTexture("output", input);
            
            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.OpacityShift;
    
        
        private struct Input : IKernelInput
        {
            private Vector4 strengthParams;
            private float amount;
            private float balance;

            public Input(Vector4 strengthParams, float amount, float balance)
            {
                this.strengthParams = strengthParams;
                this.amount = amount;
                this.balance = balance;
            }

            public string GetName() => "opacity_shift_in";

            public int GetSize() => sizeof(float) * 6;
        }
    }
}