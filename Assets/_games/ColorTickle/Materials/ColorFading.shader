Shader "ColorTickle/ColorFading"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BaseColor ("Base color", Color) = (1, 1, 1, 1)
		_FadingPerSecond ("Fading speed", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _FadingPerSecond;
			fixed4 _BaseColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
			    float fadingValue = (_FadingPerSecond * _Time.y);


			    col.rgb = _BaseColor.rgb * fadingValue * _BaseColor.a * fadingValue + col.rgb * col.a * (1 - _BaseColor.a * fadingValue);

				col.a = col.a * fadingValue + _BaseColor.a * (1 - col.a * fadingValue);

				return col;
			}
			ENDCG
		}
	}
}
