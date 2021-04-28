using UnityEngine;

namespace Background
{
    // TODO: Remove when menu item no longer needed...
    [CreateAssetMenu(menuName = "Background/Pipeline", fileName = "NewPipeline", order = 50)]
    public class Pipeline : ScriptableObject
    {
        [SerializeField] private LineOcclusionFeature lineOcclusionFeature;

        public void Execute(RenderTexture line, RenderTexture wash)
        {
            RenderLine(line);
            RenderWash(wash);
        }

        private void RenderLine(RenderTexture lineTexture)
        {
            if (lineTexture is null)
                Debug.LogError("Pipeline executed with null line texture!");
            
            lineOcclusionFeature.input = lineTexture;
            lineOcclusionFeature.Execute();
        }

        private void RenderWash(RenderTexture washTexture)
        {
            if (washTexture is null)
                Debug.LogError("Pipeline executed with null wash texture!");
        }
    }
}
