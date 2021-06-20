using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ColorDrop.Particle
{
    public interface IColorDropParticle
    {
        void BeginParticle(ColorDropParticleAttributes attributes);
    }

    public class ColorDropParticle : MonoBehaviour, IColorDropParticle
    {
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Material meshMaterial;

        public void BeginParticle(ColorDropParticleAttributes attributes)
        {

        }
    }

    [Serializable]
    public struct ColorDropParticleAttributes
    {
        public Mesh dropmesh;
        public RenderTexture renderTexture;
        public Material dropMaterial;
        public Color defaultColor;
    }
}
