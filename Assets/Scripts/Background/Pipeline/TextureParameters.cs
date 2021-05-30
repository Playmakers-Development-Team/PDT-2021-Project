using System;
using UnityEngine;

namespace Background.Pipeline
{
    /// <summary>
    /// Represents a Texture2D, with additional tilling and offset information.
    /// </summary>
    [Serializable]
    public struct TextureParameters
    {
        [SerializeField] private Texture2D texture;
        [SerializeField] private Vector2 tiling;
        [SerializeField] private Vector2 offset;

        public Texture2D Texture => texture;
        public Vector4 Parameters => new Vector4(tiling.x, tiling.y, offset.x, offset.y);
    }
}
