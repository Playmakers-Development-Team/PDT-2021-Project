using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VFX.VFX
{
    public class Splat : MonoBehaviour
    {
        private void Start()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y,
                Random.Range(-180.0f, 180.0f));
        }
    }
}
