using System.Collections.Generic;
using Background;
using UnityEngine;

namespace Managers
{
    public class BackgroundManager : IManager
    {
        private static readonly List<RenderTexture> textures = new List<RenderTexture>();
        private static readonly List<ComputeBuffer> buffers = new List<ComputeBuffer>();
        
        
        public static Pipeline ActivePipeline { get; private set; }


        public static void MarkToRelease(RenderTexture texture)
        {
            textures.Add(texture);
        }

        public static void MarkToRelease(ComputeBuffer buffer)
        {
            buffers.Add(buffer);
        }

        private static void ReleaseTextures()
        {
            foreach (RenderTexture texture in textures)
                texture.Release();
        }

        private static void ReleaseBuffers()
        {
            foreach (ComputeBuffer buffer in buffers)
                buffer.Release();
        }

        private static void Release()
        {
            ReleaseTextures();
            ReleaseBuffers();
        }

        public static void Execute(RenderTexture line, RenderTexture wash)
        {
            Execute(Settings.GlobalPipeline, line, wash);
        }

        public static void Execute(Pipeline pipeline, RenderTexture line, RenderTexture wash)
        {
            ActivePipeline = pipeline;

            ActivePipeline.Execute(line, wash);
            Release();
            
            ActivePipeline = null;
        }
    }
}
