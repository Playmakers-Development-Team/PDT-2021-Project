using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Clears a <see cref="FeatureTexture"/>.
    /// </summary>
    public class ClearFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
    
        public override void Execute()
        {
            input.Pull();
            input.Texture.Release();
            input.Texture.Create();
        }

        protected override int GetKernelIndex() => -1;
    }
}