using Background.Pipeline.Features;

namespace Background.Pipeline
{
    /// <summary>
    /// Enum for each <see cref="Feature"/> that requires a corresponding shader kernel, with the int value matching the kernel index.
    /// </summary>
    public enum KernelIndex
    {
        LineOcclusion,
        Displacement,
        OpacityExtraction,
        EdgeDetection,
        JumpFlood,
        ColourSeparation,
        OpacityShift,
        Kuwahara,
        EdgePigment,
        Bump,
        HueShiftFeature,
        SaturationFeature,
        LineTexture
    }
}
