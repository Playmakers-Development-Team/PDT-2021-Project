using System.Collections.Generic;
using UnityEngine;

namespace Background
{
    // TODO: Remove when menu item no longer needed...
    [CreateAssetMenu(menuName = "Background/Pipeline", fileName = "NewPipeline", order = 50)]
    public class Pipeline : ScriptableObject
    {
        [SerializeField] private List<Feature> features = new List<Feature>();

        private readonly Dictionary<string, RenderTexture> featureTextures =
            new Dictionary<string, RenderTexture>();

        public void Execute(RenderTexture line, RenderTexture wash)
        {
            featureTextures.Add("line", line);
            featureTextures.Add("wash", wash);

            foreach (Feature feature in features)
            {
                if (feature.IsActive)
                    feature.Execute();
            }
            
            featureTextures.Clear();
        }

        public RenderTexture GetTexture(string key) =>
            featureTextures.ContainsKey(key) ? featureTextures[key] : null;
    }
}
