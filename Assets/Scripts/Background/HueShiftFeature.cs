using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class HueShiftFeature : Feature
    {
        [SerializeField] private FeatureTexture input;

        [SerializeField] private TextureParameters strengthMap;

        [SerializeField, Range(0f, 1f)] private float amount;
        [SerializeField, Range(-1f, 1f)] private float balance;
        
        
        public override void Execute()
        {
            input.Pull();

            SetInput(new Input(strengthMap.Parameters, amount, balance));
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_tex1", strengthMap.Texture);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", input);
            
            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.Texture.width / 8, input.Texture.height / 8, 1);
        }

        protected override int GetKernelIndex() => (int) KernelIndex.HueShiftFeature;
        
        
        private struct Input : IKernelInput
        {
            private Vector4 strengthParams;
            private float amount;
            private float balance;

            public Input(Vector4 strengthParams, float amount, float balance)
            {
                this.strengthParams = strengthParams;
                this.amount = amount;
                this.balance = balance;
            }
            
            public string GetName() => "hue_shift_in";

            public int GetSize() => sizeof(float) * 6;
        }
    }
}
