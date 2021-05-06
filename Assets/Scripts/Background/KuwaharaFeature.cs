using Managers;
using UnityEngine;

namespace Background
{
    public class KuwaharaFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private Vector2 radius;
        
        
        public override void Execute()
        {
            input.Pull();
            
            // TODO: This could probably be made into a function.
            RenderTexture output = new RenderTexture(input);
            output.Create();
            BackgroundManager.MarkToRelease(output);

            SetInput(new Input(radius));
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);
            
            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), output.width / 8, output.height / 8, 1);

            Graphics.Blit(output, input);
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
