using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class CopyFeature : Feature
    {
        [SerializeField] private FeatureTexture source;
        [SerializeField] private FeatureTexture destination;

        
        public override void Execute()
        {
            source.Pull();

            if (destination.Exists())
            {
                destination.Pull();
            }
            else
            {
                destination.Texture = new RenderTexture(source.Texture);
                destination.Texture.Create();
                BackgroundManager.MarkToRelease(destination);

                destination.Push();
            }
            
            Graphics.Blit(source, destination);
        }

        protected override int GetKernelIndex() => -1;
    }
}
