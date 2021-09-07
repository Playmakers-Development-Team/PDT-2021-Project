Shader "VFX/Paint Stroke"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1, 1, 1, 1)
        _Line ("Line Colour", Color) = (1, 1, 1, 1)
        
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
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
            float _NoiseStrength;


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
                /*float start_taper_value = (i.uv.x - _StartTaper) / (max(1.0 - _StartTaper, 0.0001));
                start_taper_value = saturate(start_taper_value);
                    
                start_taper_value = sqrt(pow(start_taper_value, 2) + pow(smoothstep(_StartTaperSize * 2 - 1, 1.0, abs(i.uv.y  * 2 - 1)), 2));
                start_taper_value = saturate(start_taper_value);
                
                float end_taper_value = 1.0 - saturate(i.uv.x / max(0.0001, _EndTaper));
                end_taper_value = sqrt(pow(end_taper_value, 2) + pow(smoothstep(_EndTaperSize, 1.0, abs(i.uv.y  * 2 - 1)), 2));

                end_taper_value = sqrt(pow(end_taper_value, 2) + pow(smoothstep(_EndTaperSize * 2 - 1, 1.0, abs(i.uv.y  * 2 - 1)), 2));
                end_taper_value = saturate(end_taper_value);
                

                float taper_value = i.uv.x > 0.5 ? start_taper_value : end_taper_value;*/

                
                float4 noise_sample = tex2D(_NoiseTex, i.noise_uv);
                float y_deviation = noise_sample.r;
                y_deviation *= max(0.001, 0.5 * _NoiseStrength);

                float alpha = 0.5 - abs(0.5 - i.uv.x) - y_deviation;
                alpha = smoothstep(0, 0.05, saturate(alpha));

                float outline_mask = alpha;
                outline_mask = smoothstep(0.5, 0.54, outline_mask);

                float y = abs(i.uv.y * 2.0 - 1.0);
                outline_mask = min(outline_mask, 1.0 - smoothstep(0.8, 0.81, y));

                float3 colour = lerp(_Line.rgb, _Albedo.rgb, outline_mask);
                
                return float4(colour, smoothstep(0, .1, alpha));
            }
            ENDCG
        }
    }
}
