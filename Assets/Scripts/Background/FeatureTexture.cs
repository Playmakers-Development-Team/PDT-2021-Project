using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class FeatureTexture
    {
        [SerializeField] private string name;
        
        public RenderTexture Texture { get; private set; }

        public void Find()
        {
            Texture = BackgroundManager.ActivePipeline.GetTexture(name);
        }

        public static implicit operator RenderTexture(FeatureTexture f) => f.Texture;
    }
}