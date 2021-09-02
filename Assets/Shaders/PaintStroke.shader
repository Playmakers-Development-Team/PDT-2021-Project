Shader "VFX/Paint Stroke"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1, 1, 1, 1)
        
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
                float noise_sample = tex2D(_NoiseTex, i.uv);

                float y_deviation = noise_sample;
                clip(0.5 - abs(0.5 - i.uv.x) - y_deviation * (0.5 * _NoiseStrength + 0.001));
                
                return _Albedo;
            }
            ENDCG
        }
    }
}
