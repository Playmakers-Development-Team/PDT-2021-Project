using System;
using UnityEngine;

namespace ColorDrop.Particle
{
    public interface IColorDropParticle
    {
        void BeginParticle(ColorDropParticleAttributes attributes, IColorDropParticleRenderer particleRend);
        Vector3 GetParticlePosition();
        GameObject GetParticleGameObject();
        void EndParticle();
    }

    public class ColorDropParticle : MonoBehaviour, IColorDropParticle
    {
        // Private Fields
        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock shaderPropertyBlock;
        private IColorDropParticleRenderer particleRenderer;

        public void BeginParticle(ColorDropParticleAttributes attributes, IColorDropParticleRenderer particleRend)
        {
            meshRenderer = this.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = attributes.dropMaterial;
            shaderPropertyBlock = new MaterialPropertyBlock();
            particleRenderer = particleRend;

            // Modifies texture property block preventing shape overriding of the material
            meshRenderer.GetPropertyBlock(shaderPropertyBlock);
            shaderPropertyBlock.SetTexture("_BaseMap", attributes.renderTexture);
            shaderPropertyBlock.SetTexture("_DetailDiffuse", attributes.textureDetail);
            shaderPropertyBlock.SetTexture("_PaperTexture", attributes.texturePattern);
            shaderPropertyBlock.SetFloat("_BeginTime", Time.time);
            meshRenderer.SetPropertyBlock(shaderPropertyBlock);

            transform.localScale *= attributes.particleScale;
            meshRenderer.sharedMaterial.SetTexture("_BaseMap", attributes.renderTexture);
        }

        public GameObject GetParticleGameObject()
        {
            return gameObject;
        }

        public Vector3 GetParticlePosition()
        {
            return transform.position;
        }

        public void EndParticle()
        {
            particleRenderer.OnParticleRemoval();
            Destroy(gameObject);
        }
    }

    [Serializable]
    public struct ColorDropParticleAttributes
    {
        public float particleScale;
        public RenderTexture renderTexture;
        public Material dropMaterial;
        public Color defaultColor;
        public Color primaryColorSample;
        public Texture textureDetail;
        public Texture texturePattern;
    }
}
