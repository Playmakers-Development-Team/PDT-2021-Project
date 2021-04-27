using System;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class DisplacementFeature : Feature
    {
        [HideInInspector] public RenderTexture input;
        
        public Texture2D texture;
        public Vector4 textureParams;
        
        public float amount;
        
        
        protected override bool IsReady() => !(input is null) && !(texture is null);

        protected override int GetKernelIndex() => 1;

        public override void Execute()
        {
            base.Execute();

            ComputeBuffer buffer = SetInput(new Input(textureParams, amount));

            RenderTexture output = new RenderTexture(input.descriptor)
            {
                filterMode = input.filterMode
            };
            Graphics.CopyTexture(input, output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_displacement", texture);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", input);
            
            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.width / 8, input.height / 8, 1);

            // TODO: Figure out a smart way to get textures and buffers you want to keep for a bit
            // TODO:    out of the function... Maybe 'Settings' could come in handy? Or a manager!
            
            Graphics.CopyTexture(output, input);
            output.Release();
            
            buffer.Release();
        }
        

        private struct Input : IKernelInput
        {
            private Vector4 textureParams;
            private float amount;

            public Input(Vector4 textureParams, float amount)
            {
                this.textureParams = textureParams;
                this.amount = amount;
            }

            public string GetName() => "displacement_in";
            
            public int GetSize() => sizeof(float) * 5;
        }
    }
}
