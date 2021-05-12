using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class JumpFloodFeature : Feature
    {
        [SerializeField] private FeatureTexture edge;
        [SerializeField] private FeatureTexture output;

        public override void Execute()
        {
            edge.Pull();

            if (output.Exists())
            {
                output.Pull();
            }
            else
            {
                output.Texture = new RenderTexture(edge);
                output.Texture.Create();
                BackgroundManager.MarkToRelease(output);
            }

            Graphics.Blit(edge, output);

            SetTexture("output", output);

            Input input = new Input(new Vector2Int(output.Texture.width, output.Texture.height));
            while (input.Width > 1 || input.Height > 1)
            {
                input.StepSize = new Vector2Int(Mathf.Max(1, input.Width / 2), Mathf.Max(1, input.Height / 2));
                Graphics.Blit(output, edge);

                SetTexture("_input", edge);
                ComputeBuffer buffer = SetInput(input);
                
                Dispatch(output.Width, output.Height);
                
                buffer.Release();
            }
            
            output.Push();
        }

        protected override int GetKernelIndex() => (int) KernelIndex.JumpFlood;
        
        private struct Input : IKernelInput
        {
            public Vector2Int StepSize { get; set; }

            public int Width => StepSize.x;
            public int Height => StepSize.y;

            public Input(Vector2Int stepSize)
            {
                StepSize = stepSize;
            }
            
            public string GetName() => "jump_flood_in";

            public int GetSize() => sizeof(int) * 2;
        }
    }
}
