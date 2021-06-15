using System;
using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Generates an edge texture based on pixel kernel greyscale value.
    /// </summary>
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

            SetInput(new Input(radius, threshold));
            
            output.Texture = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                format = RenderTextureFormat.ARGBHalf
            };
            output.Texture.Create();
            BackgroundManager.MarkToRelease(output);

            SetTexture("_input", input);
            SetTexture("output", output);

            Dispatch(input.Width, input.Height);

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
