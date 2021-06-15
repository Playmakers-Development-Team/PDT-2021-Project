using System;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Applies uv displacement.
    /// </summary>
    [Serializable]
    public class DisplacementFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private TextureParameters strengthMap;
        
        [SerializeField] private float amount;
        

        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(strengthMap.Parameters, amount));

            RenderTexture output = new RenderTexture(input)
            {
                filterMode = input.Texture.filterMode
            };
            output.Create();
            Graphics.Blit(input, output);
            BackgroundManager.MarkToRelease(output);

            SetTexture("_tex1", strengthMap.Texture);
            SetTexture("_input", output);
            SetTexture("output", input);

            Dispatch(input.Width, input.Height);
        }
        
        protected override int GetKernelIndex() => (int) KernelIndex.Displacement;
        

        private struct Input : IKernelInput
        {
            private Vector4 textureParams;
            private float amount;

            public Input(Vector4 textureParams, float amount)
            {
                this.textureParams = textureParams;
                this.amount = amount;
            }

            public string GetName() => "displacement_in";
            
            public int GetSize() => sizeof(float) * 5;
        }
    }
}
