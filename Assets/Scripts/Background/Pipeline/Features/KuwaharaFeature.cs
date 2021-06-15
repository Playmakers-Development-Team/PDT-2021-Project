using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Applies a Kuwahara filter to a texture.
    /// </summary>
    public class KuwaharaFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private Vector2 radius;
        
        
        public override void Execute()
        {
            input.Pull();
            
            RenderTexture copy = new RenderTexture(input);
            copy.Create();
            Graphics.Blit(input, copy);
            BackgroundManager.MarkToRelease(copy);

            SetInput(new Input(radius));
            SetTexture("_input", copy);
            SetTexture("output", input);
            
            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.Kuwahara;
        
        
        private struct Input : IKernelInput
        {
            private Vector2 radius;

            public Input(Vector2 radius)
            {
                this.radius = radius;
            }

            public string GetName() => "kuwahara_in";

            public int GetSize() => sizeof(float) * 2;
        }
    }
}
