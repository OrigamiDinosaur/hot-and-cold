Shader "Hidden/Apache/FX/Screen Tint" {
	
	Properties {
		_Tint("Tint", Color) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" { }
	}

	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _Tint;
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;

			float4 frag(v2f_img i) : COLOR {
				
				float2 screenUV = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
				float4 tex = tex2D(_MainTex, screenUV);
				
				return fixed4(lerp(clamp(0, 1, tex.rgb), _Tint.rgb, _Tint.a), tex.a * _Tint.a);
			}

			ENDCG
		}
	}
}