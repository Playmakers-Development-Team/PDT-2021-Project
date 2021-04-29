﻿using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class DisplacementFeature : Feature
    {
        public FeatureTexture input;
        
        public Texture2D texture;
        public Vector4 textureParams;
        
        public float amount;
        
        
        protected override int GetKernelIndex() => (int) KernelIndex.Displacement;

        public override void Execute()
        {
            input.Pull();
            
            ComputeBuffer buffer = SetInput(new Input(textureParams, amount));
            BackgroundManager.MarkToRelease(buffer);

            RenderTexture output = new RenderTexture(input.Texture.descriptor)
            {
                filterMode = input.Texture.filterMode
            };
            output.Create();
            BackgroundManager.MarkToRelease(output);

            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_displacement", texture);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "_input", input);
            Settings.BackgroundCompute.SetTexture(GetKernelIndex(), "output", output);

            Settings.BackgroundCompute.Dispatch(GetKernelIndex(), input.Texture.width / 8,
                input.Texture.height / 8, 1);
            Graphics.CopyTexture(output, input);
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