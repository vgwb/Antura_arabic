Shader "Hidden/VignettingSimple" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	
	half _Intensity;
	fixed4 _Color;

	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;

		return o;
	} 
	
	fixed4 frag(v2f i) : COLOR {
		half2 coords = i.uv;
		half2 uv = i.uv;
		
		coords = (coords - 0.5) * 2.0;		
		half coordDot = dot (coords,coords);
		fixed4 color = tex2D (_MainTex, uv);	 

		half mask = 1.0 - coordDot * _Intensity * 0.1; 
		
		return lerp(_Color, color, mask);
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off	
} 