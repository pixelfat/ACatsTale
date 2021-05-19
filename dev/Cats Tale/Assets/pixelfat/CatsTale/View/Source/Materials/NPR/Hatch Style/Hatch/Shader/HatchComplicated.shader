// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "NPR Hatch Effect/Hatch Complicated" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_Hatch0Tex ("Hatch 0 Tex", 2D) = "white" {}
		_Hatch1Tex ("Hatch 1 Tex", 2D) = "white" {}
		_Hatch2Tex ("Hatch 2 Tex", 2D) = "white" {}
		_Hatch3Tex ("Hatch 3 Tex", 2D) = "white" {}
		_Hatch4Tex ("Hatch 4 Tex", 2D) = "white" {}
		_Hatch5Tex ("Hatch 5 Tex", 2D) = "white" {}
		_HatchScale ("Hatch Scale", Float) = 1
		_Intensity ("Hatch Intensity", Range(0, 1)) = 1
		_RimPower ("Rim Power", Range(0.1, 16)) = 8
		_Shininess ("Shininess", Range(1, 256)) = 128
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
			#pragma multi_compile _ NHE_INVERSE_RIM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex, _Hatch0Tex, _Hatch1Tex, _Hatch2Tex, _Hatch3Tex, _Hatch4Tex, _Hatch5Tex;
			float4 _MainTex_ST, _HatchColor;
			float _Intensity, _RimPower, _Shininess, _HatchScale;

			struct v2f
			{
				float4 pos  : SV_POSITION;
				float4 tex  : TEXCOORD0;   // .xy is hatch uv, .zw is base uv
				float3 norm : TEXCOORD1;
				float3 lit  : TEXCOORD2;
				float3 view : TEXCOORD3;
				LIGHTING_COORDS(4, 5)
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = float4(v.texcoord.xy, TRANSFORM_TEX(v.texcoord, _MainTex).xy);
				o.norm = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
				o.lit = WorldSpaceLightDir(v.vertex);
				o.view = WorldSpaceViewDir(v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				float3 N = normalize(i.norm);
				float3 L = normalize(i.lit);
				float3 V = normalize(i.view);
				float3 H = normalize(L + V);
				
				float ndl = saturate(dot(N, L));
				float rim = saturate(dot(N, V));
				float ndh = saturate(dot(N, H));
				rim = pow(rim, _RimPower);
#if NHE_INVERSE_RIM
				rim = 1.0 - rim;
#endif
				float spec = pow(ndh, _Shininess);
				float shading = (ndl + rim + spec) * _LightColor0 * 0.4;

				float4 c;
                float step = 1.0 / 6.0;
				float2 uvHatch = i.tex.xy * _HatchScale;
				
				float4 hatch5Color = tex2D(_Hatch5Tex, uvHatch);
				float4 hatch4Color = tex2D(_Hatch4Tex, uvHatch);
				float4 hatch3Color = tex2D(_Hatch3Tex, uvHatch);
				float4 hatch2Color = tex2D(_Hatch2Tex, uvHatch);
				float4 hatch1Color = tex2D(_Hatch1Tex, uvHatch);
				float4 hatch0Color = tex2D(_Hatch0Tex, uvHatch);
				
                if (shading <= step)
				{
					c = lerp(hatch5Color, hatch4Color, 6.0 * shading);
				}
				if (shading > step && shading <= 2.0 * step)
				{
					c = lerp(hatch4Color, hatch3Color, 6.0 * (shading - step));
				}
				if (shading > 2.0 * step && shading <= 3.0 * step)
				{
					c = lerp(hatch3Color, hatch2Color, 6.0 * (shading - 2.0 * step));
				}
				if (shading > 3.0 * step && shading <= 4.0 * step)
				{
					c = lerp(hatch2Color, hatch1Color, 6.0 * (shading - 3.0 * step));
				}
				if (shading > 4.0 * step && shading <= 5.0 * step)
				{
					c = lerp(hatch1Color, hatch0Color, 6.0 * (shading - 4.0 * step));
				}
				if (shading > 5.0 * step)
				{
					c = lerp(hatch0Color, 1.0, 6.0 * (shading - 5.0 * step));
				}
				float4 hatch = lerp(_HatchColor, 1.0, c.r);
				float4 base = tex2D(_MainTex, i.tex.zw) * _Intensity;
				return hatch * base * LIGHT_ATTENUATION(i);
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
