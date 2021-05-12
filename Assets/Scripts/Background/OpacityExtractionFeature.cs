using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class OpacityExtractionFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField, Range(-1f, 1f)] private float exposure;
        
        
        public override void Execute()
        {
            input.Pull();

            RenderTexture copy = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = input.Texture.filterMode
            };
            copy.Create();
            Graphics.Blit(input, copy);
            BackgroundManager.MarkToRelease(copy);

            SetTexture("_input", copy);
            SetTexture("output", input);
            
            SetInput(new Input(exposure));

            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.OpacityExtraction;
        
        private struct Input : IKernelInput
        {
            private float exposure;

            
            public Input(float exposure)
            {
                this.exposure = exposure;
            }

            
            public string GetName() => "opacity_extraction_in";

            public int GetSize() => sizeof(float);
        }
    }
}
