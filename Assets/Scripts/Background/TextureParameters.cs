using System;
using UnityEngine;

namespace Background
{
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
