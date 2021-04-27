Shader "Custom/Universal_Render_Pipeline/ColorDrop_SurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

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

            // Lighting and shadow keywords
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
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Macros.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
            };

            struct VertexOutput
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float3 positionWS       : TEXCOORD0;
                float2 uv               : TEXCOORD1;
                float3 normalWS         : TEXCOORD2;
                float4 positionCS       : SV_POSITION;
            };

            uniform float4 _Color;

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

            float3 GetViewDirectionFromPosition(float positionWS)
            {
                return normalize(GetCameraPositionWS() - positionWS);
            }

            VertexOutput vert(Attributes input) 
            {
                VertexOutput output;

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
            
            float4 frag(VertexOutput input) : SV_TARGET
            {
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float4 color = baseMap * _Color;

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS.xyz);
                Light light = GetMainLight(shadowCoord);

                float3 diffuse = LightingLambert(light.color, light.direction, input.normalWS);
                return float4(color.rgb * diffuse * light.shadowAttenuation, color.a);


                /*
                // Initialise information
                InputData lightingInput;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = NormalizeNormalPerPixel(input.normalWS);
                //lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);

                float3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).rgb;

                return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1);*/
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
