using System;
using Managers;
using UnityEngine;

namespace Background
{
    /// <summary>
    /// Represents a RenderTexture, incorporating <see cref="Pipeline"/> functionality so that it can be used by multiple <see cref="Feature"/>s.
    /// </summary>
    [Serializable]
    public class FeatureTexture
    {
        [SerializeField] protected string name;

        private RenderTexture texture;

        /// <summary>
        /// The name assigned to this texture.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// The width of the texture. Returns <c>-1</c> if <c>null</c>.
        /// </summary>
        public int Width => Texture ? Texture.width : -1;
        /// <summary>
        /// The height of the texture. Returns <c>-1</c> if <c>null</c>.
        /// </summary>
        public int Height => Texture ? Texture.height : -1;
        /// <summary>
        /// The RenderTexture itself.
        /// </summary>
        /// <exception cref="Exception"></exception>
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


        /// <summary>
        /// Register the texture with the <see cref="BackgroundManager"/>.
        /// </summary>
        public void Push() => BackgroundManager.SetTexture(this);

        /// <summary>
        /// Retrieve the texture from the <see cref="BackgroundManager"/>.
        /// </summary>
        public void Pull() => Texture = BackgroundManager.GetTexture(Name);

        /// <summary>
        /// Check if a FeatureTexture with <see cref="Name"/> has already been registered with the <see cref="BackgroundManager"/>.
        /// </summary>
        /// <returns></returns>
        public bool Exists() => BackgroundManager.ContainsTexture(Name);

        /// <summary>
        /// Check if <see cref="Texture"/> has been registered for release with the <see cref="BackgroundManager"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsMarked() => BackgroundManager.IsMarked(Texture);
        

        public static implicit operator RenderTexture(FeatureTexture f) => f.Texture;
    }
}