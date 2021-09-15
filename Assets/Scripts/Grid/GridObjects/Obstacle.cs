using UnityEngine;

namespace Grid.GridObjects
{
    public class Obstacle : GridObject
    {
        [SerializeField] protected new Renderer renderer;
        
        public Renderer Renderer => renderer;
    }
}