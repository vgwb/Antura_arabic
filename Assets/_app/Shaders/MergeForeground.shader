// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MergeForeground" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Foreground ("foreground (RGB)", 2D) = "white" {}
		_T("t", float) = 1
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _Foreground;

		half _T;
				
		uniform half4 _MainTex_TexelSize;
		
		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert (appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = half4(v.texcoord.xy,1,1);

			return o; 
		}

		half4 frag ( v2f i ) : SV_Target
		{
			half2 uv = i.uv.xy;
			
			half4 fore = tex2D(_Foreground, i.uv);
			
			half4 color = fore.a*fore + (1-fore.a)*tex2D(_MainTex, i.uv);

			return lerp(fixed4(0,0,0,1),color, _T);

		}	
					
	ENDCG
	
	SubShader {
	  ZTest Off Cull Off ZWrite Off Blend Off

	Pass {		
		ZTest Always
		Cull Off
				
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
		ENDCG
		}	
	}	

	FallBack Off
}
