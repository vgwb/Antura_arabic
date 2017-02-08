// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1,x:35761,y:33007,varname:node_1,prsc:2|emission-532-RGB,alpha-563-OUT,clip-2366-OUT;n:type:ShaderForge.SFN_Pi,id:441,x:33699,y:33854,varname:node_441,prsc:2;n:type:ShaderForge.SFN_Slider,id:528,x:35378,y:33593,ptovrint:False,ptlb:Slope,ptin:_Slope,varname:node_897,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.5152014,max:1;n:type:ShaderForge.SFN_Vector1,id:531,x:35203,y:33335,varname:node_531,prsc:2,v1:0;n:type:ShaderForge.SFN_Color,id:532,x:35311,y:32677,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4043,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Vector3,id:558,x:34713,y:32941,varname:node_558,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Lerp,id:559,x:35080,y:32958,varname:node_559,prsc:2|A-5552-OUT,B-558-OUT,T-617-OUT;n:type:ShaderForge.SFN_ComponentMask,id:563,x:35319,y:32989,varname:node_563,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-559-OUT;n:type:ShaderForge.SFN_Slider,id:617,x:34634,y:33072,ptovrint:False,ptlb:Transparency,ptin:_Transparency,varname:node_8401,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ConstantClamp,id:689,x:32999,y:33606,varname:node_689,prsc:2,min:-1,max:1|IN-528-OUT;n:type:ShaderForge.SFN_Rotator,id:7382,x:34662,y:33470,varname:node_7382,prsc:2|UVIN-7923-OUT,PIV-735-OUT,ANG-9253-OUT;n:type:ShaderForge.SFN_If,id:2366,x:35509,y:33290,varname:node_2366,prsc:2|A-689-OUT,B-531-OUT,GT-7628-OUT,EQ-7628-OUT,LT-8103-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8964,x:34890,y:33470,varname:node_8964,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-7382-UVOUT;n:type:ShaderForge.SFN_Step,id:7628,x:35087,y:33500,varname:node_7628,prsc:2|A-8964-OUT,B-2328-OUT;n:type:ShaderForge.SFN_Vector1,id:2328,x:34918,y:33661,varname:node_2328,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector2,id:735,x:34242,y:33823,varname:node_735,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Append,id:3324,x:34035,y:34274,varname:node_3324,prsc:2|A-3535-OUT,B-7977-OUT;n:type:ShaderForge.SFN_Multiply,id:7923,x:34312,y:33479,varname:node_7923,prsc:2|A-7136-UVOUT,B-6769-OUT;n:type:ShaderForge.SFN_Rotator,id:7136,x:34044,y:33474,varname:node_7136,prsc:2|UVIN-1405-UVOUT,PIV-1798-OUT,ANG-441-OUT;n:type:ShaderForge.SFN_TexCoord,id:1405,x:33660,y:33473,varname:node_1405,prsc:2,uv:0;n:type:ShaderForge.SFN_Append,id:1798,x:33570,y:33659,varname:node_1798,prsc:2|A-7977-OUT,B-689-OUT;n:type:ShaderForge.SFN_Vector1,id:7977,x:33260,y:33893,varname:node_7977,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:57,x:34332,y:34099,varname:node_57,prsc:2|A-4320-UVOUT,B-3324-OUT;n:type:ShaderForge.SFN_Vector1,id:5889,x:34901,y:34276,varname:node_5889,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Step,id:8103,x:35106,y:34099,varname:node_8103,prsc:2|A-2413-OUT,B-5889-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2413,x:34901,y:34099,varname:node_2413,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-5880-UVOUT;n:type:ShaderForge.SFN_Rotator,id:5880,x:34677,y:34099,varname:node_5880,prsc:2|UVIN-57-OUT,PIV-735-OUT,ANG-9253-OUT;n:type:ShaderForge.SFN_Vector2,id:6500,x:33786,y:34112,varname:node_6500,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Rotator,id:4320,x:34055,y:34098,varname:node_4320,prsc:2|UVIN-1405-UVOUT,PIV-6500-OUT,ANG-441-OUT;n:type:ShaderForge.SFN_Negate,id:3535,x:33508,y:34246,varname:node_3535,prsc:2|IN-689-OUT;n:type:ShaderForge.SFN_Append,id:6769,x:34033,y:33666,varname:node_6769,prsc:2|A-689-OUT,B-7977-OUT;n:type:ShaderForge.SFN_Pi,id:168,x:34523,y:33801,varname:node_168,prsc:2;n:type:ShaderForge.SFN_Vector1,id:9861,x:34506,y:33929,varname:node_9861,prsc:2,v1:4;n:type:ShaderForge.SFN_Divide,id:9253,x:34693,y:33801,varname:node_9253,prsc:2|A-168-OUT,B-9861-OUT;n:type:ShaderForge.SFN_Vector1,id:7915,x:33156,y:32967,varname:node_7915,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:7717,x:33467,y:32995,varname:node_7717,prsc:2|A-7915-OUT,B-2684-OUT;n:type:ShaderForge.SFN_Slider,id:2684,x:32999,y:33070,ptovrint:False,ptlb:GradientScale,ptin:_GradientScale,varname:_GradientScale_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_TexCoord,id:2751,x:33202,y:32646,varname:node_2751,prsc:2,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:1351,x:34016,y:32724,varname:node_1351,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-7318-OUT;n:type:ShaderForge.SFN_Multiply,id:7318,x:33741,y:32784,varname:node_7318,prsc:2|A-6670-UVOUT,B-7717-OUT;n:type:ShaderForge.SFN_Append,id:6896,x:34253,y:32722,varname:node_6896,prsc:2|A-1351-OUT,B-1351-OUT,C-1351-OUT;n:type:ShaderForge.SFN_Vector3,id:9237,x:34421,y:32827,varname:node_9237,prsc:2,v1:1,v2:1,v3:1;n:type:ShaderForge.SFN_ToggleProperty,id:1125,x:34253,y:32541,ptovrint:False,ptlb:UseGradient,ptin:_UseGradient,varname:node_1125,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Rotator,id:6670,x:33539,y:32650,varname:node_6670,prsc:2|UVIN-2751-UVOUT,ANG-2105-OUT;n:type:ShaderForge.SFN_Pi,id:2105,x:33235,y:32819,varname:node_2105,prsc:2;n:type:ShaderForge.SFN_Vector1,id:5676,x:34253,y:32625,varname:node_5676,prsc:2,v1:1;n:type:ShaderForge.SFN_If,id:5552,x:34691,y:32619,varname:node_5552,prsc:2|A-1125-OUT,B-5676-OUT,GT-9237-OUT,EQ-6896-OUT,LT-9237-OUT;proporder:528-532-617-2684-1125;pass:END;sub:END;*/

Shader "Shader Forge/AreaShading" {
    Properties {
        _Slope ("Slope", Range(-1, 1)) = 0.5152014
        _Color ("Color", Color) = (0,1,1,1)
        _Transparency ("Transparency", Range(0, 1)) = 0
        _GradientScale ("GradientScale", Range(0, 1)) = 1
        [MaterialToggle] _UseGradient ("UseGradient", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float _Slope;
            uniform float4 _Color;
            uniform float _Transparency;
            uniform float _GradientScale;
            uniform fixed _UseGradient;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float node_689 = clamp(_Slope,-1,1);
                float node_2366_if_leA = step(node_689,0.0);
                float node_2366_if_leB = step(0.0,node_689);
                float node_9253 = (3.141592654/4.0);
                float2 node_735 = float2(0.5,0.5);
                float node_5880_ang = node_9253;
                float node_5880_spd = 1.0;
                float node_5880_cos = cos(node_5880_spd*node_5880_ang);
                float node_5880_sin = sin(node_5880_spd*node_5880_ang);
                float2 node_5880_piv = node_735;
                float node_441 = 3.141592654;
                float node_4320_ang = node_441;
                float node_4320_spd = 1.0;
                float node_4320_cos = cos(node_4320_spd*node_4320_ang);
                float node_4320_sin = sin(node_4320_spd*node_4320_ang);
                float2 node_4320_piv = float2(0.5,0.5);
                float2 node_4320 = (mul(i.uv0-node_4320_piv,float2x2( node_4320_cos, -node_4320_sin, node_4320_sin, node_4320_cos))+node_4320_piv);
                float node_7977 = 1.0;
                float2 node_5880 = (mul((node_4320*float2((-1*node_689),node_7977))-node_5880_piv,float2x2( node_5880_cos, -node_5880_sin, node_5880_sin, node_5880_cos))+node_5880_piv);
                float node_7382_ang = node_9253;
                float node_7382_spd = 1.0;
                float node_7382_cos = cos(node_7382_spd*node_7382_ang);
                float node_7382_sin = sin(node_7382_spd*node_7382_ang);
                float2 node_7382_piv = node_735;
                float node_7136_ang = node_441;
                float node_7136_spd = 1.0;
                float node_7136_cos = cos(node_7136_spd*node_7136_ang);
                float node_7136_sin = sin(node_7136_spd*node_7136_ang);
                float2 node_7136_piv = float2(node_7977,node_689);
                float2 node_7136 = (mul(i.uv0-node_7136_piv,float2x2( node_7136_cos, -node_7136_sin, node_7136_sin, node_7136_cos))+node_7136_piv);
                float2 node_7382 = (mul((node_7136*float2(node_689,node_7977))-node_7382_piv,float2x2( node_7382_cos, -node_7382_sin, node_7382_sin, node_7382_cos))+node_7382_piv);
                float node_7628 = step(node_7382.g,0.5);
                clip(lerp((node_2366_if_leA*step(node_5880.r,0.5))+(node_2366_if_leB*node_7628),node_7628,node_2366_if_leA*node_2366_if_leB) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                float node_5552_if_leA = step(_UseGradient,1.0);
                float node_5552_if_leB = step(1.0,_UseGradient);
                float3 node_9237 = float3(1,1,1);
                float node_2105 = 3.141592654;
                float node_6670_ang = node_2105;
                float node_6670_spd = 1.0;
                float node_6670_cos = cos(node_6670_spd*node_6670_ang);
                float node_6670_sin = sin(node_6670_spd*node_6670_ang);
                float2 node_6670_piv = float2(0.5,0.5);
                float2 node_6670 = (mul(i.uv0-node_6670_piv,float2x2( node_6670_cos, -node_6670_sin, node_6670_sin, node_6670_cos))+node_6670_piv);
                float2 node_7717 = float2(1.0,_GradientScale);
                float node_1351 = (node_6670*node_7717).g;
                float3 node_6896 = float3(node_1351,node_1351,node_1351);
                fixed4 finalRGBA = fixed4(finalColor,lerp(lerp((node_5552_if_leA*node_9237)+(node_5552_if_leB*node_9237),node_6896,node_5552_if_leA*node_5552_if_leB),float3(0,0,0),_Transparency).r);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float _Slope;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float node_689 = clamp(_Slope,-1,1);
                float node_2366_if_leA = step(node_689,0.0);
                float node_2366_if_leB = step(0.0,node_689);
                float node_9253 = (3.141592654/4.0);
                float2 node_735 = float2(0.5,0.5);
                float node_5880_ang = node_9253;
                float node_5880_spd = 1.0;
                float node_5880_cos = cos(node_5880_spd*node_5880_ang);
                float node_5880_sin = sin(node_5880_spd*node_5880_ang);
                float2 node_5880_piv = node_735;
                float node_441 = 3.141592654;
                float node_4320_ang = node_441;
                float node_4320_spd = 1.0;
                float node_4320_cos = cos(node_4320_spd*node_4320_ang);
                float node_4320_sin = sin(node_4320_spd*node_4320_ang);
                float2 node_4320_piv = float2(0.5,0.5);
                float2 node_4320 = (mul(i.uv0-node_4320_piv,float2x2( node_4320_cos, -node_4320_sin, node_4320_sin, node_4320_cos))+node_4320_piv);
                float node_7977 = 1.0;
                float2 node_5880 = (mul((node_4320*float2((-1*node_689),node_7977))-node_5880_piv,float2x2( node_5880_cos, -node_5880_sin, node_5880_sin, node_5880_cos))+node_5880_piv);
                float node_7382_ang = node_9253;
                float node_7382_spd = 1.0;
                float node_7382_cos = cos(node_7382_spd*node_7382_ang);
                float node_7382_sin = sin(node_7382_spd*node_7382_ang);
                float2 node_7382_piv = node_735;
                float node_7136_ang = node_441;
                float node_7136_spd = 1.0;
                float node_7136_cos = cos(node_7136_spd*node_7136_ang);
                float node_7136_sin = sin(node_7136_spd*node_7136_ang);
                float2 node_7136_piv = float2(node_7977,node_689);
                float2 node_7136 = (mul(i.uv0-node_7136_piv,float2x2( node_7136_cos, -node_7136_sin, node_7136_sin, node_7136_cos))+node_7136_piv);
                float2 node_7382 = (mul((node_7136*float2(node_689,node_7977))-node_7382_piv,float2x2( node_7382_cos, -node_7382_sin, node_7382_sin, node_7382_cos))+node_7382_piv);
                float node_7628 = step(node_7382.g,0.5);
                clip(lerp((node_2366_if_leA*step(node_5880.r,0.5))+(node_2366_if_leB*node_7628),node_7628,node_2366_if_leA*node_2366_if_leB) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
