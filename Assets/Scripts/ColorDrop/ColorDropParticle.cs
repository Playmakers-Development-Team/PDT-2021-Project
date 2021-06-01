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
        public void BeginParticle(ColorDropParticleAttributes attributes)
        {

        }
    }

    [Serializable]
    public struct ColorDropParticleAttributes
    {
        public Color defaultColor;
    }
}
