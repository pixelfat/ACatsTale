// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NPR Hatch Effect/Background" {
	Properties {
		_MainTex ("Background", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			struct v2f
			{
				float4 pos : POSITION;
				float4 posscr : TEXCOORD0;		
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posscr = ComputeScreenPos(o.pos);  
				return o;
			}
			float4 frag (v2f i) : COLOR  
			{
				float2 uv = i.posscr.xy / i.posscr.w;
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}