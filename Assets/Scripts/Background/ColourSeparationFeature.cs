using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class ColourSeparationFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField] private FeatureTexture distanceField;
        
        [SerializeField] private TextureParameters strengthMap;

        [SerializeField, Range(0f, 1f)] private float amount;
        
        public override void Execute()
        {
            input.Pull();
            distanceField.Pull();

            SetTexture("output", input);
            SetTexture("_tex1", distanceField);
            SetTexture("_tex2", strengthMap.Texture);

            SetInput(new Input(strengthMap.Parameters, amount));
            
            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.ColourSeparation;
        
        
        private struct Input : IKernelInput
        {
            private Vector4 strengthParams;
            private float amount;

            public Input(Vector4 strengthParams, float amount)
            {
                this.strengthParams = strengthParams;
                this.amount = amount;
            }

            public string GetName() => "colour_separation_in";

            public int GetSize() => sizeof(float) * 5;
        }
    }
}
