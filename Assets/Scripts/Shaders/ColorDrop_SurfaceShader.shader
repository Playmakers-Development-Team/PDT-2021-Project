Shader "Custom/Universal_Render_Pipeline/ColorDrop_SurfaceShader"
{
    Properties
    {
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap ("Base Map", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 1.0
        _Smoothing("Smoothing", Range(0, 0.5)) = 0

    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha // Traditional Alpha Blending
        //Blend One One

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
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Macros.hlsl"

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

            uniform float4 _Color;
            uniform float _Smoothing;
            uniform float _Cutoff;

            TEXTURE2D(_BaseMap); 
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

            float3 GetViewDirectionFromPosition(float positionWS)
            {
                return normalize(GetCameraPositionWS() - positionWS);
            }

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
            
            float4 frag(VertexOutput input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                //float4 sdf = tex2D(_BaseMap, input.uv);
                //float3 color = float3(1,1,1);
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                //color = color * sdf.rgb;
                //float alpha = _Color.a * tex.a;
                //AlphaDiscard(alpha, _Cutoff);

                //color *= alpha;
                //return float4(color, alpha);
                //float alpha = smoothstep(0.5 - _Smoothing, 0.5 + _Smoothing, tex.a); // Smoothing alpha value
                //AlphaDiscard(alpha, _Cutoff);

                /*float4 c;
                c.rgb = input.color.rgb;
                c.a = input.color.a * alpha;*/
                float distance = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
                float alpha = smoothstep(0.5 - _Smoothing, 0.5 + _Smoothing, distance);

                float4 c;
                //c.rgb = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                c.rgb = float3(1,1,1);
                c.a = alpha;
                return c;
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
