Shader "Julian/Cutoff"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Color("Undownloaded Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 0
	}
		SubShader
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 100

			Pass
		{
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
			// make fog work
	#pragma multi_compile_fog

	#include "UnityCG.cginc"

			struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		sampler2D _TransitionTex;
		float4 _MainTex_ST;
		fixed4 _Color;
		float _Cutoff;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// sample the transition texture
			fixed4 trans = tex2D(_TransitionTex, i.uv);

			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv);
			
			// Check if this part is loaded already
			if (trans.b < _Cutoff)
				return col = _Color;
			
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
				ENDCG
			}
		}
}