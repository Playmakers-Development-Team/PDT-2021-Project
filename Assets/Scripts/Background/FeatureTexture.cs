using System;
using Managers;
using UnityEngine;

namespace Background
{
    [Serializable]
    public class FeatureTexture
    {
        [SerializeField] protected string name;

        private RenderTexture texture;
        
        public string Name => name;
        public int Width => Texture ? Texture.width : -1;
        public int Height => Texture ? Texture.height : -1;
        public RenderTexture Texture
        {
            get
            {
                if (texture is null)
                {
                    throw new Exception($"Feature Texture with name '{name}' could not be found in pipeline." +
                                        "Make sure you're calling Pull(), and the texture exists.");
                }

                return texture;
            }
            set => texture = value;
        }


        public void Push() => BackgroundManager.SetTexture(this);

        public void Pull() => Texture = BackgroundManager.GetTexture(Name);

        public bool Exists() => BackgroundManager.ContainsTexture(Name);

        public bool IsMarked() => BackgroundManager.IsMarked(Texture);
        

        public static implicit operator RenderTexture(FeatureTexture f) => f.Texture;
    }
}