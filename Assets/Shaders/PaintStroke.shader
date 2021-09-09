Shader "VFX/Paint Stroke"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1, 1, 1, 1)
        _Line ("Line Colour", Color) = (1, 1, 1, 1)
        
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        
        _LineWeight ("Line Weight", Range(0, 1))  = 0.1
        _LineSharpness ("Line Sharpness", Range(0, 1))  = 0.1
        
        _ParallelDisplacement ("Parallel Displacement", Range(0, 1))  = 0.1
        _PerpendicularDisplacement ("Perpendicular Displacement", Range(0, 1))  = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Structs
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 noise_uv : TEXCOORD1;
            };


            // VARIABLES
            // Colours
            float4 _Albedo;
            float4 _Line;

            // Noise Texture
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

			float _Aspect;

            float _LineWidth;
            float _LineWeight;
            float _LineSharpness;

            float _ParallelDisplacement;
            float _PerpendicularDisplacement;


            // Programs
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.noise_uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Store uv coordinates in 1 -> 0 -> 1 range.
                float2 edge_mask = i.uv * 2 - 1;
                edge_mask = abs(edge_mask);

                // Displace UV coordinates.
                float2 noise_value = float2(
					tex2D(_NoiseTex, float2(0, i.noise_uv.y)).r,
					tex2D(_NoiseTex, float2(i.noise_uv.x * _Aspect, 0)).r
				);

                float2 displacement;
                displacement.x = saturate(noise_value.x + lerp(-1, 1, _ParallelDisplacement));
                displacement.y = saturate(noise_value.y + lerp(-1, 1, _PerpendicularDisplacement));

                edge_mask += displacement;

                float alpha = max(edge_mask.x, edge_mask.y) > 1 ? 0 : 1;

                // Apply value smoothing.
                float2 smooth_min = float2(1.0 - _LineWeight / _Aspect, 1.0 - _LineWeight);
                float2 smooth_max = lerp(smooth_min, 1.0, _LineSharpness);
                
                edge_mask = smoothstep(smooth_min, smooth_max, edge_mask);
                
                float edge_value = max(edge_mask.x, edge_mask.y);
                
                return float4(lerp(_Albedo, _Line, edge_value).rgb, alpha);
            }
            ENDCG
        }
    }
}
