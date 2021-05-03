using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class LineOcclusionFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        

        protected override int GetKernelIndex() => (int) KernelIndex.LineOcclusion;

        public override void Execute()
        {
            input.Pull();
            
            RenderTexture output = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = input.Texture.filterMode
            };
            output.Create();
            BackgroundManager.MarkToRelease(output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8,
                1);
            
            Graphics.CopyTexture(output, input);
        }
    }
}
