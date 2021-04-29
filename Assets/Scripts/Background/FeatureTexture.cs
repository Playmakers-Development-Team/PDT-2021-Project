using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class FeatureTexture
    {
        [SerializeField] protected string name;
        
        
        public string Name => name;
        public RenderTexture Texture { get; protected set; }


        public void Push() => BackgroundManager.SetTexture(this);

        public void Pull() => Texture = BackgroundManager.GetTexture(Name);
        

        public static implicit operator RenderTexture(FeatureTexture f) => f.Texture;
    }
}