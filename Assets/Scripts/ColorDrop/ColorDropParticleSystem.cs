using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorDrop.Particle;

namespace ColorDrop
{
    public interface IColorDropParticleRenderer
    {

    }

    public class ColorDropParticleSystem : MonoBehaviour, IColorDropParticleRenderer
    {

        [HideInInspector] public ColorDropParticleAttributes particleAttributes;

        [HideInInspector] public float startDelay;
        [HideInInspector] public float startRotation;

        [HideInInspector] public Color defaultColor;
        [HideInInspector] public float initialAlpha;
        [HideInInspector] public Vector2 dropScale;

        [HideInInspector] public Material material;
        [HideInInspector] public int sortLayer;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}