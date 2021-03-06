using UnityEngine;

namespace Background.Pipeline.Features
{
    /// <summary>
    /// Applies the effect of an uneven surface such as watercolour paper.
    /// </summary>
    public class BumpFeature : Feature
    {
        [SerializeField] private FeatureTexture input;
        [SerializeField] private FeatureTexture flood;
        
        [SerializeField] private TextureParameters bumpMap;
        [SerializeField] private TextureParameters strengthMap;
        [SerializeField, Range(0f, 1f)] private float amount;

        public override void Execute()
        {
            input.Pull();
            flood.Pull();

            SetInput(new Input(bumpMap.Parameters, strengthMap.Parameters, amount));
            
            SetTexture("output", input);
            SetTexture("_tex1", flood);
            SetTexture("_tex2", bumpMap.Texture);
            SetTexture("_tex3", strengthMap.Texture);

            Dispatch(input.Width, input.Height);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.Bump;
        
        
        private struct Input : IKernelInput
        {
            private Vector4 bumpParams;
            private Vector4 strengthParams;
            private float amount;

            public Input(Vector4 bumpParams, Vector4 strengthParams, float amount)
            {
                this.bumpParams = bumpParams;
                this.strengthParams = strengthParams;
                this.amount = amount;
            }

            public string GetName() => "bump_in";

            public int GetSize() => sizeof(float) * 9;
        }
    }
}
