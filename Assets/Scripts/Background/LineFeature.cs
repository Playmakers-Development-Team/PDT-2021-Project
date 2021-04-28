using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class LineFeature : Feature
    {
        [HideInInspector] public RenderTexture input;
        
        
        protected override bool IsReady() => !(input is null);

        protected override int GetKernelIndex() => 0;

        public override void Execute()
        {
            base.Execute();

            RenderTexture output = new RenderTexture(input.descriptor)
            {
                filterMode = input.filterMode
            };
            output.Create();

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.width / 8, input.height / 8,
                1);
            
            Graphics.CopyTexture(output, input);
            output.Release();
        }
    }
}
