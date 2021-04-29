using Background;

public class ClearFeature : Feature
{
    public FeatureTexture input;
    
    public override void Execute()
    {
        input.Find();
        input.Texture.Release();
        input.Texture.Create();
    }

    protected override int GetKernelIndex() => -1;
}