Shader "Custom/Universal_Render_Pipeline/ColorDropEffect"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1,1,1,1)
        _ColorSampleB("Color Sample B", Color) = (1,1,1,1)
        _ColorSampleC ("Color Sample C", Color) = (1,1,1,1)
        _SubLayerColor ("Sub Layer Color", Color) = (1,1,1,1)

        [PerRendererData] _BaseMap("Base Map", 2D) = "white" {}
        [PerRendererData] _BeginTime("BeginTime", Float) = 0
        _LerpDuration("Fade Lerp Duration", Float) = 1
        _Smoothing("Smoothing", Range(0, 0.5)) = 0
        _NoiseStrength("Noise Strength", Float) = 0
        _NoiseScale("Noise Scale", Float) = 30
        _CircleRadius("Circle Radius", Float) = 1
        _DecalEdgeWidth("Decal Edge Witdh", Range(0.0, 1.0)) = 0
        _EdgeDarkness("Edge Darkness", Color) = (1,1,1,1)
        _ControlVal("Control Value", Range(0.0, 1.0)) = 1 // TEST

            /*  COLORS
            * 
            * _PrimaryColor
            * _SubColorDarkness
            * _EdgeColor
            * _BorderColor
            */

        // Border Attributes
        _BorderColor("Border Color", Color) = (1,1,1,1)
        _BorderSize("Border Size", Range(0, 0.25)) = 0.5
        _BorderSmoothing("Border Smoothing", Range(0, 0.5)) = 0

        // Texture Detail
        [PerRendererData] _DetailDiffuse("Detail Diffuse", 2D) = "white" {}
        [PerRendererData] _PaperTexture("Paper Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha // Traditional Alpha Blending
        Cull Front

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back

            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma require geometry
            #pragma target 4.5

            // Lighting and shadow keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct VertexOutput
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float3 positionWS       : TEXCOORD0;
                float2 uv               : TEXCOORD1;
                float3 normalWS         : TEXCOORD2;
                float4 color            : COLOR;
                float4 positionCS       : SV_POSITION;
            };

            // Variables 
            uniform float4 _Color;
            uniform float _BeginTime;
            uniform float _LerpDuration;
            uniform float _NoiseStrength;
            uniform float _NoiseScale;
            uniform float4 _BorderColor;
            uniform float4 _EdgeDarkness;
            uniform float _BorderSize;
            uniform float _BorderSmoothing;
            uniform float _Smoothing;
            uniform float _CircleRadius;
            uniform float _DecalEdgeWidth;
            uniform float _ControlVal;

            uniform float4 _SubLayerColor;

            // Textures
            TEXTURE2D(_DetailDiffuse);
            SAMPLER(sampler_DetailDiffuse);

            TEXTURE2D(_PaperTexture);
            SAMPLER(sampler_PaperTexture);

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // Buffers
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _DetailDiffuse_ST;
                float4 _PaperTexture_ST;
            CBUFFER_END

            // --------------------------------------------
            //                  MATH
            // --------------------------------------------

            float3 GetViewDirectionFromPosition(float positionWS)
            {
                return normalize(GetCameraPositionWS() - positionWS);
            }

            // --------------------------------------------
           //                  GRADIENT NOISE 
           // --------------------------------------------

            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            {
                Out = unity_gradientNoise(UV * Scale) + 0.5;
            }

            // --------------------------------------------
           //                  SIMPLE NOISE 
           // --------------------------------------------

            inline float unity_noise_randomValue(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            inline float unity_noise_interpolate(float a, float b, float t)
            {
                return (1.0 - t) * a + (t * b);
            }

            inline float unity_valueNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = unity_noise_randomValue(c0);
                float r1 = unity_noise_randomValue(c1);
                float r2 = unity_noise_randomValue(c2);
                float r3 = unity_noise_randomValue(c3);

                float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
                float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
                float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
            {
                float t = 0.0;

                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3 - 0));
                t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                t += unity_valueNoise(float2(UV.x * Scale / freq, UV.y * Scale / freq)) * amp;

                Out = t;
            }

            // --------------------------------------------
            //               NOISE PROCESSING
            // --------------------------------------------

            void Unity_Step_float4(float4 Edge, float4 In, out float4 Out)
            {
                Out = step(Edge, In);
            }

            void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Blend_Dodge_float4(float4 Base, float4 Blend, float Opacity, out float4 Out)
            {
                Out = Base / (1.0 - Blend);
                Out = lerp(Base, Out, Opacity);
            }

            void Unity_Blend_Subtract_float4(float4 Base, float4 Blend, float Opacity, out float4 Out)
            {
                Out = Base - Blend;
                Out = lerp(Base, Out, Opacity);
            }

            float4 DarkNoiseEdgeProcess(float4 steppedNoiseA, float4 steppedNoiseB, float edgeSerrator) {
                
                float4 edge = (_EdgeDarkness * steppedNoiseA) * steppedNoiseB;
                edge *= pow(edgeSerrator, 0.56);
                edge.a = 0;
                return edge;
            }


            // Vert node processor
            VertexOutput vert(Attributes input)
            {
                VertexOutput output = (VertexOutput)0;

                // Convert to world space
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;

                // Convert normal to world space;
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;

                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            // Graphic fragment section
            float4 frag(VertexOutput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float4 detail = SAMPLE_TEXTURE2D(_DetailDiffuse, sampler_DetailDiffuse, input.uv);
                float4 paper = SAMPLE_TEXTURE2D(_PaperTexture, sampler_PaperTexture, input.uv);
                float distance = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
                float alpha = smoothstep(0.5 - _Smoothing, 0.5 + _Smoothing, distance);

                float borderCenter = 0.5 + _BorderSize;
                float border = 1 - smoothstep(borderCenter - _BorderSmoothing, borderCenter + _BorderSmoothing, distance);

                float4 c = float4(1, 1, 1, 0);
                c.rgba = lerp(c.rgba, _BorderColor, border);
                c.a = alpha;

                float controlVal = lerp(1, 0, saturate((_Time.y - _BeginTime) / _LerpDuration));
                controlVal *= -1;

                float4 combinedTex;
                Unity_Blend_Dodge_float4(detail, paper, 1.0, combinedTex);
                combinedTex *= _Color;

                // Noise Generation
                float noise = 0;
                Unity_SimpleNoise_float(input.uv, 30, noise);

                // Edge Noise
                float edgeNoise;
                Unity_SimpleNoise_float(input.uv, 242, edgeNoise);

                // Sub-Noise layer
                float subNoise;
                Unity_SimpleNoise_float(input.uv, 2, subNoise);
                float4 subColor = _SubLayerColor;
                subColor.rgb *= subNoise;

                float4 compositeColor;
                Unity_Blend_Subtract_float4(combinedTex, subColor, 0.8, compositeColor);

                // Processes the visible of the noise
                float4 stepA = float4(1, 1, 1, 1);
                Unity_Step_float4(noise, 1 + controlVal, stepA);
                float4 baseColor = stepA * compositeColor;

                // Processes the secondary stepped noise before extracting the edge
                float4 stepB = float4(1, 1, 1, 1);
                Unity_Step_float4(noise, (1 - _DecalEdgeWidth) + controlVal, stepB);
                stepB = abs(float4(1, 1, 1, 1) - stepB);

                // Subtracted the edge from the patterned noise result for the final color
                float4 color = baseColor - DarkNoiseEdgeProcess(stepA, stepB, edgeNoise);
                c *= color;
                c.a = stepA.a * alpha;

                return c;
            }
            ENDHLSL
        }   
    }
        FallBack "Diffuse"
}