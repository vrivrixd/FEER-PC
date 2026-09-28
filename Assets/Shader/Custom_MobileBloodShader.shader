// PORT: rebuilt from the compiled shader in the Android build (AssetRipper exported an opaque placeholder,
// so the blood on the lens was drawn as a solid image). Same render state and math as the original:
// alpha blended, alpha = r * g * b of the texture (dark areas are see-through), color darkened by
// _Darkness and shaded by the _Distortion relief map.
Shader "Custom/MobileBloodShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Distortion ("Normalmap", 2D) = "black" {}
		_Relief ("Relief Value", Range(0, 2)) = 1.5
		_Darkness ("Darkness", Range(0, 100)) = 10
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Distortion;
			fixed4 _Color;
			half _Relief;
			half _Darkness;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color.rgb = saturate(1.0 - _Darkness) * _Color.rgb;
				o.color.a = _Color.a;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 tex = tex2D(_MainTex, i.texcoord).rgb;
				fixed4 col;
				col.a = tex.r * tex.g * tex.b * i.color.a;
				half relief = 1.0 - (tex2D(_Distortion, i.texcoord).r * 2.0 - 1.0) * _Relief;
				col.rgb = relief * tex * i.color.rgb;
				return col;
			}
			ENDCG
		}
	}
}
