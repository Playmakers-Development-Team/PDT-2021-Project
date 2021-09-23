using UnityEngine;

namespace Grid.GridObjects
{
    public class Obstacle : GridObject
    {
        public Renderer Renderer { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();

            Renderer = transform.parent.GetComponentInChildren<Renderer>();
        }
    }
}