Shader "Battlehub/RTHandles/VertexColor" {
	Properties
	{
		_ZWrite("ZWrite", Float) = 0.0
		_ZTest("ZTest", Float) = 0.0
		_Cull("Cull", Float) = 0.0
	}
		SubShader
	{
		Tags{ "Queue" = "Geometry+5" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Pass
		{
			Cull[_Cull]
			ZTest Off
			ZWrite Off
		
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
				float4 color: COLOR;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 color: COLOR;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.color = input.color;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return input.color;
			}	

			ENDCG
		}
	}
}