using Managers;
using UnityEngine;

namespace Background.Pipeline.Features
{
    public class SDFFeature : Feature
    {
        [SerializeField] private FeatureTexture mask;
        [SerializeField] private FeatureTexture jumpFlood;

        [SerializeField] private FeatureTexture output;
        
        public override void Execute()
        {
            mask.Pull();
            jumpFlood.Pull();

            if (output.Exists())
            {
                output.Pull();
            }
            else
            {
                output.Texture = new RenderTexture(jumpFlood.Width, jumpFlood.Height, jumpFlood.Texture.depth,
                    RenderTextureFormat.ARGBHalf);
                output.Texture.Create();
                BackgroundManager.MarkToRelease(output);
            }

            Graphics.Blit(jumpFlood, output);

            SetTexture("output", output);
            SetTexture("_input", mask);

            Dispatch(Threads.x, Threads.y, Threads.z);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.SDFFeature;
    }
}
