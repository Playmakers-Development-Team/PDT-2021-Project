using System;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Varies the hue of the input texture according to a strength map.
    /// </summary>
    [Serializable]
    public class HueShiftFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private TextureParameters strengthMap;

        [SerializeField, Range(0f, 1f)] private float amount;
        [SerializeField, Range(-1f, 1f)] private float balance;
        [SerializeField, Range(0f, 1f)] private float valueInfluence = 1f;
        [SerializeField] private float boost = 1f;
        
        
        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(strengthMap.Parameters, amount, balance, valueInfluence, boost));
            SetTexture("_tex1", strengthMap.Texture);
            SetTexture("output", input);
            
            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.HueShiftFeature;
        
        
        private struct Input : IKernelInput
        {
            private Vector4 strengthParams;
            private float amount;
            private float balance;
            private float valueInfluence;
            private float boost;

            public Input(Vector4 strengthParams, float amount, float balance, float valueInfluence, float boost)
            {
                this.strengthParams = strengthParams;
                this.amount = amount;
                this.balance = balance;
                this.valueInfluence = valueInfluence;
                this.boost = boost;
            }
            
            public string GetName() => "hue_shift_in";

            public int GetSize() => sizeof(float) * 8;
        }
    }
}
