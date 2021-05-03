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

            RenderTexture output = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = input.Texture.filterMode
            };
            output.Create();
            BackgroundManager.MarkToRelease(output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);
            
            BackgroundManager.MarkToRelease(SetInput(new Input(exposure)));

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8, 1);
            Graphics.CopyTexture(output, input);
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
