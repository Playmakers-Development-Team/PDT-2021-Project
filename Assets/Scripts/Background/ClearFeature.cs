using Background;
using UnityEngine;

public class ClearFeature : Feature
{
    [SerializeField] private FeatureTexture input;
    
    public override void Execute()
    {
        input.Pull();
        
        input.Texture.Release();
        input.Texture.Create();
    }

    protected override int GetKernelIndex() => -1;
}