using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class LineOcclusionFeature : Feature
    {
        [HideInInspector] public RenderTexture input;
        
        
        protected override bool IsReady() => !(input is null);

        protected override int GetKernelIndex() => (int) KernelIndex.LineOcclusion;

        public override void Execute()
        {
            base.Execute();

            RenderTexture output = new RenderTexture(input.descriptor)
            {
                filterMode = input.filterMode
            };
            output.Create();
            BackgroundManager.MarkToRelease(output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.width / 8, input.height / 8,
                1);
            
            Graphics.CopyTexture(output, input);
        }
    }
}
