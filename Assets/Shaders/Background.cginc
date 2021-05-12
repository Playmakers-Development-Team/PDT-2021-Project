// Textures
RWTexture2D<float4> output;

sampler2D _input;
sampler2D _tex1;
sampler2D _tex2;
sampler2D _tex3;

// Static members
static float root_two = 1.41421356237;

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

static const float sobel_x[9] = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
static const float sobel_y[9] = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };

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

struct jump_flood_input
{
    int2 step_size;
};

struct colour_separation_input
{
    float4 strength_params;
    float amount;
};

struct opacity_shift_input
{
    float4 strength_params;
    float amount;
    float balance;
};

struct kuwahara_input
{
    float2 radius;
};

struct edge_pigment_input
{
    float4 strength_params;
    float amount;
    float exponent;
};

struct bump_input
{
    float4 bump_params;
    float4 strength_params;
    float amount;
};

struct hue_shift_input
{
    float4 strength_params;
    float amount;
    float balance;
};

// Buffers
StructuredBuffer<displacement_input> displacement_in;
StructuredBuffer<opacity_extraction_input> opacity_extraction_in;
StructuredBuffer<edge_detection_input> edge_detection_in;
StructuredBuffer<jump_flood_input> jump_flood_in;
StructuredBuffer<colour_separation_input> colour_separation_in;
StructuredBuffer<opacity_shift_input> opacity_shift_in;
StructuredBuffer<kuwahara_input> kuwahara_in;
StructuredBuffer<edge_pigment_input> edge_pigment_in;
StructuredBuffer<bump_input> bump_in;
StructuredBuffer<hue_shift_input> hue_shift_in;

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

float get_pixel_angle(float2 resolution, float2 uv)
{
    float2 gradient = 0;
    int i = 0;

    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            const float2 offset = float2(x / resolution.x, y / resolution.y);
            const float3 pixel_color = tex2Dlod(_input, float4(uv + offset, 0, 0)).rgb;
            const float value = dot(pixel_color, float3(0.3, 0.59, 0.11));

            gradient.x += value * sobel_x[i];
            gradient.y += value * sobel_y[i];
            i++;
        }
    }

    return atan(gradient.x / gradient.y);
}

float4 kuwahara_kernel(float2 resolution, float2 uv, float4 range, float2x2 rotation_matrix, out float variance)
{
    variance = 0;
    
    float4 mean = 0;
    uint samples = 0;
    
    for (int x = range.x; x <= range.y; x++)
    {
        for (int y = range.z; y <= range.w; y++)
        {
            const float2 offset = mul(float2(x / resolution.x, y / resolution.y), rotation_matrix);
            const float4 pixel_color = tex2Dlod(_input, float4(uv + offset, 0, 0));
            
            mean += pixel_color;
            variance += dot(pixel_color, pixel_color);
            
            samples++;
        }
    }
    
    mean /= samples;
    variance = variance / samples - dot(mean, mean);
    
    variance = length(variance);
    
    return mean;
}

// Thanks: https://www.shadertoy.com/view/MsS3Wc
float3 to_rgb(float3 hsv)
{
    float3 rgb = clamp( abs(fmod(hsv.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
    rgb = rgb*rgb*(3.0-2.0*rgb);
    return hsv.z * lerp( float3(1,1,1), rgb, hsv.y);
}

float3 to_hsv(float3 rgb)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}