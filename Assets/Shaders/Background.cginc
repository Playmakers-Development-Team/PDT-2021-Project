// Textures
RWTexture2D<float4> output;

sampler2D _input;
sampler2D _displacement;

// Kernel input structs
struct displacement_input
{
    float4 texture_params;
    float amount;
};

// Buffers
StructuredBuffer<displacement_input> displacement_in;

// Functions
float2 get_output_resolution()
{
    uint2 resolution;
    output.GetDimensions(resolution.x, resolution.y);
    return resolution;
}

float2 pixel_to_uv(float2 pixel, const float2 resolution)
{
    return pixel / (resolution - 1.0);
}

float4 sample_params(sampler2D tex, float2 uv, float4 params)
{
    float4 samp = tex2Dlod(tex, float4((uv + params.zw) * params.xy, 0, 0));
    return samp;
}