using System.Collections.Generic;
using Background;
using Background.Pipeline;
using UnityEngine;

namespace Managers
{
    public class BackgroundManager : Manager
    {
        private static readonly List<RenderTexture> textures = new List<RenderTexture>();
        private static readonly List<ComputeBuffer> buffers = new List<ComputeBuffer>();
        private static readonly Dictionary<string, RenderTexture> featureTextures =
            new Dictionary<string, RenderTexture>();

        public BackgroundCamera BackgroundCamera { get; set; }

        public static Pipeline ActivePipeline { get; private set; }


        public static void MarkToRelease(RenderTexture texture)
        {
            if (!textures.Contains(texture))
                textures.Add(texture);
        }

        public static void MarkToRelease(ComputeBuffer buffer)
        {
            if (!buffers.Contains(buffer))
                buffers.Add(buffer);
        }

        private static void ReleaseTextures()
        {
            foreach (RenderTexture texture in textures)
            {
                if (texture)
                    texture.Release();
            }
            
        }

        private static void ReleaseBuffers()
        {
            foreach (ComputeBuffer buffer in buffers)
            {
                if (!(buffer is null))
                    buffer.Release();
            }
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
            featureTextures.Clear();
            
            TryAddTexture("line", line);
            TryAddTexture("wash", wash);

            ActivePipeline = pipeline;

            ActivePipeline.Execute();
            Graphics.SetRenderTarget(null);
            
            Release();
            
            ActivePipeline = null;
            featureTextures.Clear();
        }

        private static void TryAddTexture(string key, RenderTexture texture)
        {
            if (ContainsTexture(key))
                featureTextures[key] = texture;
            else
                featureTextures.Add(key, texture);
        }

        public static void SetTexture(FeatureTexture texture)
        {
            TryAddTexture(texture.Name, texture.Texture);
        }

        public static RenderTexture GetTexture(string key) =>
            ContainsTexture(key) ? featureTextures[key] : null;

        public static bool ContainsTexture(string key) => featureTextures.ContainsKey(key);

        public static bool IsMarked(RenderTexture texture) => textures.Contains(texture);

        public void Render()
        {
            if (BackgroundCamera is null)
            {
                Debug.Log($"Cannot call function " + 
                    $"{nameof(BackgroundManager)}.{nameof(Render)} before " +
                    $"{nameof(BackgroundCamera)} is initialised.");
                return;
            }
            
            BackgroundCamera.Render();
        }
    }
}
