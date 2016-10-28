Shader "Battlehub/SplineEditor/SSBillboard" {
	Properties
	{
		_Color("Color", Color) = (0,0,0,1)
		_Scale("Scale", Float) = 1.0
		_ZWrite("ZWrite", Float) = 0.0
		_ZTest("ZTest", Float) = 0.0
		_Cull("Cull", Float) = 0.0
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry+1" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Pass
		{
			Cull[_Cull]
			ZTest [_ZTest]
			ZWrite [_ZWrite]
			Offset -1, -1
			Stencil
			{
				Ref 1
				Comp always
				Pass replace
			}

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
				float4 offset : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
			};

			float _Scale;
			fixed4 _Color;

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float t = unity_CameraProjection[1].y;
				float h = _ScreenParams.y;
				float orthoSize = unity_OrthoParams.y;
				
				float4 vert = mul(UNITY_MATRIX_MV, input.vertex);
				
				float dist = dot(vert.xyz, float3(0, 0, 1));
				float tan = 1.0f / t;
				float denom = lerp(dist * 7.0 * _Scale / h * tan, orthoSize * 7.0 * _Scale / h, unity_OrthoParams.w);

				output.pos = mul(UNITY_MATRIX_P, vert -
					float4(input.offset.x * denom, input.offset.y * denom, 0.0, 0.0));

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return _Color;
			}

			ENDCG
		}
	}
}