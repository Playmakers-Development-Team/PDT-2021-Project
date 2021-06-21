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

        public void BeginParticle(ColorDropParticleAttributes attributes)
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = attributes.dropMaterial;

            transform.localScale *= attributes.particleScale;
            meshRenderer.sharedMaterial.SetTexture("_BaseMap", attributes.renderTexture);
        }
    }

    [Serializable]
    public struct ColorDropParticleAttributes
    {
        public float particleScale;
        public RenderTexture renderTexture;
        public Material dropMaterial;
        public Color defaultColor;
    }
}
