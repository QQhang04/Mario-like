// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fire"
{
	Properties
	{
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_Gradient("Gradient", 2D) = "white" {}
		_NoiseSpeed("Noise Speed", Vector) = (0,0,0,0)
		_FireColor("Fire Color", Color) = (0,0,0,0)
		_Softness("Softness", Range( 0 , 1)) = 0.18
		_EmissIntensity("Emiss Intensity", Float) = 0
		_GradientEndControl("Gradient End Control", Float) = 0
		_EndEmiss("End Emiss", Range( 0 , 1)) = 0
		_FireShape("FireShape", 2D) = "white" {}
		_NoiseIntensity("Noise Intensity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _FireColor;
		uniform float _EmissIntensity;
		uniform float _EndEmiss;
		uniform sampler2D _Gradient;
		SamplerState sampler_Gradient;
		uniform float4 _Gradient_ST;
		uniform float _GradientEndControl;
		uniform sampler2D _NoiseTexture;
		SamplerState sampler_NoiseTexture;
		uniform float2 _NoiseSpeed;
		uniform float4 _NoiseTexture_ST;
		uniform float _Softness;
		uniform sampler2D _FireShape;
		SamplerState sampler_FireShape;
		uniform float _NoiseIntensity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 break41 = ( _FireColor * _EmissIntensity );
			float2 uv_Gradient = i.uv_texcoord * _Gradient_ST.xy + _Gradient_ST.zw;
			float4 tex2DNode10 = tex2D( _Gradient, uv_Gradient );
			float GradientEnd47 = ( ( 1.0 - tex2DNode10.r ) * _GradientEndControl );
			float2 uv_NoiseTexture = i.uv_texcoord * _NoiseTexture_ST.xy + _NoiseTexture_ST.zw;
			float2 panner7 = ( _Time.y * _NoiseSpeed + uv_NoiseTexture);
			float Noise23 = tex2D( _NoiseTexture, panner7 ).r;
			float4 appendResult42 = (float4(break41.r , ( break41.g + ( _EndEmiss * GradientEnd47 * Noise23 ) ) , break41.b , 0.0));
			o.Emission = appendResult42.xyz;
			float Gradient22 = tex2DNode10.r;
			float smoothstepResult19 = smoothstep( ( Noise23 - _Softness ) , Noise23 , Gradient22);
			float4 appendResult62 = (float4(( i.uv_texcoord.x + ( (-1.0 + (Noise23 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _NoiseIntensity * GradientEnd47 ) ) , i.uv_texcoord.y , 0.0 , 0.0));
			float clampResult65 = clamp( pow( tex2D( _FireShape, appendResult62.xy ).r , 4.0 ) , 0.0 , 1.0 );
			o.Alpha = ( smoothstepResult19 * clampResult65 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;96;1111;633;3840.697;1146.948;4.533586;True;False
Node;AmplifyShaderEditor.CommentaryNode;26;-2090.576,-272.4313;Inherit;False;1293.516;647.9156;Gradient;13;17;10;22;8;6;7;5;23;43;44;45;46;47;;0.0552123,0.8301887,0.01174793,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-2026.576,-199.1718;Inherit;False;0;10;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;43;-1685.697,302.9828;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;8;-1813.843,189.4843;Inherit;False;Property;_NoiseSpeed;Noise Speed;2;0;Create;True;0;0;False;0;False;0,0;0,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1817.972,53.61597;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-1754.918,-222.4313;Inherit;True;Property;_Gradient;Gradient;1;0;Create;True;0;0;False;0;False;-1;5a0c3923d79f7429eac7e34bfaedf598;5a0c3923d79f7429eac7e34bfaedf598;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;7;-1578.679,149.1822;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;46;-1392.195,-90.01762;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-1372.934,122.007;Inherit;True;Property;_NoiseTexture;Noise Texture;0;0;Create;True;0;0;False;0;False;-1;d73a6daea5b2c4da9af132755d71d22d;d73a6daea5b2c4da9af132755d71d22d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-1219.197,35.98236;Inherit;False;Property;_GradientEndControl;Gradient End Control;6;0;Create;True;0;0;False;0;False;0;-4.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1027.719,148.8184;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1171.197,-90.01762;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;67;-2411.659,461.0173;Inherit;False;1633.754;1187.979;Shape;18;63;65;55;21;24;20;25;19;60;58;56;61;57;54;59;62;53;64;;0.9889445,0,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-995.103,-151.3659;Inherit;True;GradientEnd;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-2355.586,1212.819;Inherit;True;23;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;61;-2115.897,1214.994;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-2106.897,1425.995;Inherit;False;Property;_NoiseIntensity;Noise Intensity;9;0;Create;True;0;0;False;0;False;0;-0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-2102.897,1533.996;Inherit;False;47;GradientEnd;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;-1931.897,1056.992;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-1811.897,1256.994;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-1653.897,1185.994;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;66;-1854.14,-1051.975;Inherit;False;1041.572;665.1359;Color;10;39;9;40;52;48;51;50;41;49;42;;0.1962442,0.3357311,0.8490566,1;0;0
Node;AmplifyShaderEditor.ColorNode;9;-1804.14,-1001.975;Inherit;False;Property;_FireColor;Fire Color;3;0;Create;True;0;0;False;0;False;0,0,0,0;1,0.1333039,0.004716992,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;62;-1472.369,1156.194;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1737.568,-773.3395;Inherit;False;Property;_EmissIntensity;Emiss Intensity;5;0;Create;True;0;0;False;0;False;0;10.88;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-1585.068,-579.3398;Inherit;False;47;GradientEnd;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-2303.155,534.5159;Inherit;False;23;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2361.659,628.5099;Inherit;False;Property;_Softness;Softness;4;0;Create;True;0;0;False;0;False;0.18;0.395;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1258.654,937.4286;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-1304.128,-209.845;Inherit;False;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1544.568,-924.3395;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-1591.568,-501.8397;Inherit;False;23;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1649.068,-678.3397;Inherit;False;Property;_EndEmiss;End Emiss;7;0;Create;True;0;0;False;0;False;0;0.388;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;53;-1514.03,737.7307;Inherit;True;Property;_FireShape;FireShape;8;0;Create;True;0;0;False;0;False;-1;None;1ec55d2e3c9a048acb28e4ee0aaaafb1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;-2067.931,609.284;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-2052.15,511.0173;Inherit;False;22;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;41;-1338.568,-924.3395;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.PowerNode;63;-1126.535,785.6109;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1329.068,-638.3398;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-1080.068,-717.3395;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;19;-1830.276,618.3629;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;65;-948.9066,784.435;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1040.061,545.5942;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-973.5686,-924.3395;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-349.7124,-60.72659;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Fire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;1;17;0
WireConnection;7;0;6;0
WireConnection;7;2;8;0
WireConnection;7;1;43;0
WireConnection;46;0;10;1
WireConnection;5;1;7;0
WireConnection;23;0;5;1
WireConnection;44;0;46;0
WireConnection;44;1;45;0
WireConnection;47;0;44;0
WireConnection;61;0;56;0
WireConnection;57;0;61;0
WireConnection;57;1;58;0
WireConnection;57;2;60;0
WireConnection;59;0;54;1
WireConnection;59;1;57;0
WireConnection;62;0;59;0
WireConnection;62;1;54;2
WireConnection;22;0;10;1
WireConnection;40;0;9;0
WireConnection;40;1;39;0
WireConnection;53;1;62;0
WireConnection;20;0;24;0
WireConnection;20;1;21;0
WireConnection;41;0;40;0
WireConnection;63;0;53;1
WireConnection;63;1;64;0
WireConnection;50;0;48;0
WireConnection;50;1;51;0
WireConnection;50;2;52;0
WireConnection;49;0;41;1
WireConnection;49;1;50;0
WireConnection;19;0;25;0
WireConnection;19;1;20;0
WireConnection;19;2;24;0
WireConnection;65;0;63;0
WireConnection;55;0;19;0
WireConnection;55;1;65;0
WireConnection;42;0;41;0
WireConnection;42;1;49;0
WireConnection;42;2;41;2
WireConnection;0;2;42;0
WireConnection;0;9;55;0
ASEEND*/
//CHKSM=BAF61BEB73C2FB267D34ACAA18CD3598D981AD56