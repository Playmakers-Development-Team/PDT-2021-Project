using System;
using Managers;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Converts brightness to opacity.
    /// </summary>
    [Serializable]
    public class LineOcclusionFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        

        protected override int GetKernelIndex() => (int) KernelIndex.LineOcclusion;

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

            Dispatch(input.Width, input.Height);
        }
    }
}
