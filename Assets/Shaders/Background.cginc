// Textures
RWTexture2D<float4> output;

sampler2D _input;
sampler2D _displacement;

// Static members
static float3 greyscale_constant = float3(0.3, 0.59, 0.11);

static float2 offsets[9] = {
    float2(-1, -1),
    float2(0, -1),
    float2(1, -1),
    float2(-1, 0),
    float2(0, 0),
    float2(1, 0),
    float2(-1, 1),
    float2(0, 1),
    float2(1, 1)
};

// Kernel input structs
struct displacement_input
{
    float4 texture_params;
    float amount;
};

struct opacity_extraction_input
{
    float exposure;
};

struct edge_detection_input
{
    uint radius;
    float threshold;
};

// Buffers
StructuredBuffer<displacement_input> displacement_in;
StructuredBuffer<opacity_extraction_input> opacity_extraction_in;
StructuredBuffer<edge_detection_input> edge_detection_in;

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