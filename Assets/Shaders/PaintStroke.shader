Shader "VFX/Paint Stroke"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1, 1, 1, 1)
        
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        
        _StartTaper ("Start Taper", Range(0, 1)) = 1
        _EndTaper ("End Taper", Range(0, 1)) = 1
        _TaperAmount ("Taper Amount", Range(0, 0.5)) = 1
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
            };


            // Variables
            // Main Texture
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Albedo;

            // Noise Texture
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _NoiseStrength;

            float _StartTaper;
            float _EndTaper;
            float _TaperAmount;


            // Programs
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                /*float noise_sample = tex2D(_NoiseTex, i.uv);

                float y_deviation = noise_sample;
                clip(0.5 - abs(0.5 - i.uv.x) - y_deviation * (0.5 * _NoiseStrength + 0.001));*/

                // Map i.uv.x - _StartTaper from _StartTaper, 1.0 to 
                float start_taper_point = saturate(i.uv.x - _StartTaper) / (max(1.0 - _StartTaper, 0.0001));
                float end_taper_point = 1.0 - saturate(i.uv.x / max(0.0001, _EndTaper));
                
                return float4(_Albedo.rgb, 1.0 - max(start_taper_point, end_taper_point));
            }
            ENDCG
        }
    }
}
