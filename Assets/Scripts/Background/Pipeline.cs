using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    /// <summary>
    /// A scriptable pipeline for rendering backgrounds.
    /// </summary>
    [CreateAssetMenu(menuName = "Background/Pipeline", fileName = "NewPipeline", order = 50)]
    public class Pipeline : ScriptableObject
    {
        [SerializeField] private List<Feature> features = new List<Feature>();

        /// <summary>
        /// Execute each <see cref="Feature"/> in this pipeline consecutively.
        /// </summary>
        public void Execute()
        {
            foreach (Feature feature in features)
            {
                if (feature.IsActive)
                    feature.Execute();
            }
        }
    }
}
