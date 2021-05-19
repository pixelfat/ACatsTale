// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NPR Hatch Effect/Hatch Simple" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_Hatch0Tex ("Hatch 0 Tex", 2D) = "white" {}
		_Hatch1Tex ("Hatch 1 Tex", 2D) = "white" {}
		_Hatch2Tex ("Hatch 2 Tex", 2D) = "white" {}
		_Hatch3Tex ("Hatch 3 Tex", 2D) = "white" {}
		_Hatch4Tex ("Hatch 4 Tex", 2D) = "white" {}
		_Hatch5Tex ("Hatch 5 Tex", 2D) = "white" {}
		_HatchScale ("Hatch Scale", Float) = 1
		_Intensity ("BaseColor Intensity", Range(0.5, 2)) = 1
		_HatchDensity ("Hatch Density", Range(1, 8)) = 4
		_HatchColor ("Hatch Color", Color) = (0, 0, 0, 0)
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 0)
		_OutlineWidth ("Outline Width", Float) = 0.1
		_ExpandFactor ("Outline Factor", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		Pass {
			Cull Back
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex, _Hatch0Tex, _Hatch1Tex, _Hatch2Tex, _Hatch3Tex, _Hatch4Tex, _Hatch5Tex;
			float4 _MainTex_ST, _HatchColor;
			float _Intensity, _HatchDensity, _HatchScale;
		
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;   // .xy is hatch uv, .zw is base uv
				float3 hw0 : TEXCOORD1;
				float3 hw1 : TEXCOORD2;
				LIGHTING_COORDS(3, 4)
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = float4(v.texcoord.xy * _HatchScale, TRANSFORM_TEX(v.texcoord, _MainTex).xy);
				
				float3 n = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
				float3 l = normalize(WorldSpaceLightDir(v.vertex));
				float diff = min(1, max(0, dot(l, n)));
				diff = pow(diff, _HatchDensity);
				
				float factor = diff * 6;
				float3 w0 = float3(0, 0, 0);
				float3 w1 = float3(0, 0, 0);
				if (factor > 5)
				{
					w0.x = 1;
				}
				else if (factor > 4)
				{
					w0.x = 1 - (5 - factor);
					w0.y = 1 - w0.x;
				}
				else if (factor > 3)
				{
					w0.y = 1 - (4 - factor);
					w0.z = 1 - w0.y;
				}
				else if (factor > 2)
				{
					w0.z = 1 - (3 - factor);
					w1.x = 1 - w0.z;
				}
				else if (factor > 1)
				{
					w1.x = 1 - (2 - factor);
					w1.y = 1 - w1.x;
				}
				else if (factor > 0)
				{
					w1.y = 1 - (1 - factor);
					w1.z = 1 - w1.y;
				}
				o.hw0 = w0;
				o.hw1 = w1;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				float4 h0 = tex2D(_Hatch0Tex, i.tex.xy) * i.hw0.x;
				float4 h1 = tex2D(_Hatch1Tex, i.tex.xy) * i.hw0.y;
				float4 h2 = tex2D(_Hatch2Tex, i.tex.xy) * i.hw0.z;
				float4 h3 = tex2D(_Hatch3Tex, i.tex.xy) * i.hw1.x;
				float4 h4 = tex2D(_Hatch4Tex, i.tex.xy) * i.hw1.y;
				float4 h5 = tex2D(_Hatch5Tex, i.tex.xy) * i.hw1.z;
				float4 base = tex2D(_MainTex, i.tex.zw) * _Intensity;
				float4 hatch = lerp(_HatchColor, 1, (h0 + h1 + h2 + h3 + h4 + h5).r);
				return hatch * base * _LightColor0 * LIGHT_ATTENUATION(i);
			}
			ENDCG
		}
		Pass {
			Cull Front

			CGPROGRAM
			#include "Outline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
