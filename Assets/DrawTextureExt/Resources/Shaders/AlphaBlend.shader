// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "DrawTextureExt/AlphaBlend" {
	Properties {
		_MainTex ("Texture (RGBA)", 2D) = "white" {}
	}

	Category {
		LOD 200

		Tags { 
			"Queue"="Overlay+1" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		ZWrite Off 
		Lighting Off 

		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader {
			Pass {
				SetTexture [_MainTex] {
					combine texture * primary
				}
			}
		}

		SubShader {
			pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				struct v2f {
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				
				v2f vert (appdata_t v) {
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
				
				fixed4 frag (v2f IN) : COLOR {
					return fixed4(tex2D(_MainTex, IN.texcoord) * IN.color);
				}
				ENDCG
			}
		}
	} 
}
