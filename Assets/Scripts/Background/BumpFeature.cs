using UnityEngine;

namespace Background
{
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
            
            Shader.SetTexture(GetKernelIndex(), "output", input);
            Shader.SetTexture(GetKernelIndex(), "_tex1", flood);
            Shader.SetTexture(GetKernelIndex(), "_tex2", bumpMap.Texture);
            Shader.SetTexture(GetKernelIndex(), "_tex3", strengthMap.Texture);

            Shader.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8, 1);
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
