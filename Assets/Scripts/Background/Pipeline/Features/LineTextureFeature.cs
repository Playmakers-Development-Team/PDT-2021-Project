﻿using UnityEngine;

namespace Background.Pipeline.Features
{
    public class LineTextureFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField] private FeatureTexture flood;
        [SerializeField] private TextureParameters texture;
        
        [SerializeField] private float threshold;
        [SerializeField] private float amount;

        
        public override void Execute()
        {
            flood.Pull();
            input.Pull();

            SetTexture("_input", flood);
            SetTexture("_tex1", texture.Texture);
            SetTexture("output", input);

            SetInput(new Input(texture.Parameters, threshold, amount));
            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.LineTexture;
        
        private struct Input : IKernelInput
        {
            private Vector4 textureParams;
            private float threshold;
            private float amount;


            public Input(Vector4 textureParams, float threshold, float amount)
            {
                this.textureParams = textureParams;
                this.threshold = threshold;
                this.amount = amount;
            }


            public string GetName() => "line_texture_in";

            public int GetSize() => sizeof(float) * 6;
        }
    }
}
