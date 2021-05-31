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