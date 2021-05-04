using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    [CreateAssetMenu(menuName = "Background/Pipeline", fileName = "NewPipeline", order = 50)]
    public class Pipeline : ScriptableObject
    {
        [SerializeField] private List<Feature> features = new List<Feature>();

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
