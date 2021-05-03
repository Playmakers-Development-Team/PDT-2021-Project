using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class EdgeDetectionFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField] private FeatureTexture output;
        
        [SerializeField] private uint radius;
        [SerializeField, Range(0f, 1f)] private float threshold;
        
        
        public override void Execute()
        {
            input.Pull();

            BackgroundManager.MarkToRelease(SetInput(new Input(radius, threshold)));
            
            output.Texture = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                format = RenderTextureFormat.ARGBHalf
            };
            output.Texture.Create();
            BackgroundManager.MarkToRelease(output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8, 1);

            output.Push();
        }

        protected override int GetKernelIndex() => (int) KernelIndex.EdgeDetection;
        
        private struct Input : IKernelInput
        {
            private uint radius;
            private float threshold;

            
            public Input(uint radius, float threshold)
            {
                this.radius = radius;
                this.threshold = threshold;
            }

            
            public string GetName() => "edge_detection_in";

            public int GetSize() => sizeof(uint) + sizeof(float);
        }
    }
}
