Shader "Battlehub/RTHandles/Shape" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_ZWrite("ZWrite", Float) = 0.0
		_ZTest("ZTest", Float) = 0.0
		_Cull("Cull", Float) = 0.0
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry+6" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Pass
		{
			Cull Back
			ZTest[_ZTest]
			ZWrite[_ZWrite]
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color: COLOR;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 color: COLOR;
			};

			fixed4 _Color;

			vertexOutput vert(vertexInput input)
			{
				float3 viewNorm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, input.normal));
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.color = input.color * 1.5 * dot(viewNorm, float3(0, 0, 1));
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return _Color * input.color;
			}	

			ENDCG
		}
	}
}