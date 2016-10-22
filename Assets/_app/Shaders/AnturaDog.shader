// HowTo:
// This shader should be used by antura base materials (e.g. robotic, wool, etc.)
// Add this e.g. as first in the list of renderer materials
// This uses uv2

// To be used with the next version (two uvs) of the dog!
Shader "Antura/Dog" {
	Properties{
		_MainTex("Base (RGB) Glossiness (A)", 2D) = "white" {}
		_Occlusion("Occlusion (A)", 2D) = "white" {}
		_BaseColor("Base Color", Color) = (1,1,1,1)

		_Shininess("Shininess", Range(0.03, 1)) = 0.078125
		_Specular("Specular", Range(0,1)) = 0.5

		_Emission("Emission", Color) = (0,0,0,0)
		_SpecularColor("Specular Color", Color) = (0,0,0,0)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 250

		CGPROGRAM
#pragma surface surf AnturaBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

#include "LightningSpecular.cginc"

		sampler2D _MainTex;
		sampler2D _Occlusion;

		half _Shininess;
		half _Specular;

		fixed4 _BaseColor;

		fixed3 _Emission;
		fixed3 _SpecularColor;

		struct Input {
			half2 uv2_MainTex;
			half2 uv_Occlusion;
		};

		void surf(Input IN, inout SurfaceOutputSpecularAntura o) {
			fixed4 baseColor = tex2D(_MainTex, IN.uv2_MainTex)*tex2D(_Occlusion, IN.uv_Occlusion).a;
			baseColor.rgb *= _BaseColor;
			
			o.Albedo = baseColor.rgb;
			o.Gloss = _Specular*baseColor.a;
			o.Alpha = 1;
			o.Specular = _Shininess;
			o.SpecularColor = _SpecularColor;
			o.Emission = _Emission;
		}
	ENDCG
	}

		FallBack "Mobile/VertexLit"
}
