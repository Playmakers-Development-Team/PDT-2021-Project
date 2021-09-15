Shader "Background/Paint Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DispTex ("Displacement Texture", 2D) = "grey" {}
		[IntRange] _StencilRef ("Stencil Reference Value", Range(0, 255)) = 0
		_DispAmount ("Displacement Amount", Float) = 0.1
    }
    SubShader
    {
		Stencil {
			Ref [_StencilRef]
			Comp Always
			Pass Replace
		}
		
        Blend Zero One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 displacement_uv : TEXCOORD1;
            };

            sampler2D _MainTex;

            sampler2D _DispTex;
            float4 _DispTex_ST;

			float _DispAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.displacement_uv = TRANSFORM_TEX(mul(unity_ObjectToWorld, v.vertex).xy, _DispTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 displacement_sample = tex2D(_DispTex, i.displacement_uv);
				float2 displacement = displacement_sample.xy * 2 - 1;
				displacement *= _DispAmount;

                fixed4 col = tex2D(_MainTex, i.uv + displacement);
				clip(col.a - 0.0001);
                return col;
            }
            ENDCG
        }
    }
}
