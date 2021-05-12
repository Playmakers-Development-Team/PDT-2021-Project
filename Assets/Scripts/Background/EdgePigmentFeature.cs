using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class EdgePigmentFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField] private FeatureTexture flood;
        
        [SerializeField] private TextureParameters strengthMap;

        [SerializeField] private float amount;
        [SerializeField, Range(0f, 1f)] private float exponent;
        
        public override void Execute()
        {
            input.Pull();
            flood.Pull();
            
            SetInput(new Input(strengthMap.Parameters, amount, exponent));
            
            SetTexture("output", input);
            SetTexture("_tex1", flood);
            SetTexture("_tex2", strengthMap.Texture);

            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.EdgePigment;
        
        
        private struct Input : IKernelInput
        {
            private Vector4 strengthParams;
            private float amount;
            private float exponent;


            public Input(Vector4 strengthParams, float amount, float exponent)
            {
                this.strengthParams = strengthParams;
                this.amount = amount;
                this.exponent = exponent;
            }


            public string GetName() => "edge_pigment_in";

            public int GetSize() => sizeof(float) * 6;
        }
    }
}
