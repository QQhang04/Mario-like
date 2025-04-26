// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Dissolve Easy Double"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Gradient("Gradient", 2D) = "white" {}
		_ChangeAmount("Change Amount", Range( 0 , 1)) = 0.6352941
		_EdgeRange("Edge Range", Range( 0 , 2)) = 0.09411766
		_EdgeIntensity("Edge Intensity", Float) = 1
		[Toggle(_MANUALCONTROL_ON)] _ManualControl("Manual Control", Float) = 0
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Spread("Spread", Range( 0 , 1)) = 1
		_RampTex("Ramp Tex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _MANUALCONTROL_ON
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _RampTex;
		uniform sampler2D _Gradient;
		SamplerState sampler_Gradient;
		uniform float4 _Gradient_ST;
		uniform float _ChangeAmount;
		uniform float _Spread;
		uniform sampler2D _TextureSample1;
		SamplerState sampler_TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _EdgeRange;
		uniform float _EdgeIntensity;
		SamplerState sampler_TextureSample0;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode1 = tex2D( _TextureSample0, uv_TextureSample0 );
			float2 uv_Gradient = i.uv_texcoord * _Gradient_ST.xy + _Gradient_ST.zw;
			float mulTime28 = _Time.y * 0.3;
			#ifdef _MANUALCONTROL_ON
				float staticSwitch30 = _ChangeAmount;
			#else
				float staticSwitch30 = frac( mulTime28 );
			#endif
			float Gradient23 = ( ( ( tex2D( _Gradient, uv_Gradient ).r - (-_Spread + (staticSwitch30 - 0.0) * (1.0 - -_Spread) / (1.0 - 0.0)) ) / _Spread ) * 2.0 );
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float2 panner34 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TextureSample1);
			float Noise37 = tex2D( _TextureSample1, panner34 ).r;
			float temp_output_39_0 = ( Gradient23 - Noise37 );
			float smoothstepResult19 = smoothstep( 0.0 , 1.0 , temp_output_39_0);
			float clampResult49 = clamp( ( 1.0 - ( distance( smoothstepResult19 , 0.5 ) / _EdgeRange ) ) , 0.0 , 1.0 );
			float2 appendResult46 = (float2(( 1.0 - clampResult49 ) , 0.5));
			float4 RampColor47 = tex2D( _RampTex, appendResult46 );
			float EdgeColor26 = ( clampResult49 < 0.4 ? 0.0 : clampResult49 );
			float4 lerpResult12 = lerp( tex2DNode1 , ( RampColor47 * _EdgeIntensity * tex2DNode1 ) , EdgeColor26);
			o.Emission = lerpResult12.rgb;
			o.Alpha = 1;
			clip( ( tex2DNode1.a * temp_output_39_0 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
1;96;1209;646;2136.173;-46.59062;1.003622;True;False
Node;AmplifyShaderEditor.CommentaryNode;22;-2179.181,-139.2038;Inherit;False;1089.899;804.7397;Gradient;12;23;5;2;6;4;28;29;30;41;42;43;44;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;28;-2156.669,298.0261;Inherit;False;1;0;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2334.461,453.1062;Inherit;False;Property;_Spread;Spread;9;0;Create;True;0;0;False;0;False;1;0.63;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2083.229,534.7965;Inherit;False;Property;_ChangeAmount;Change Amount;3;0;Create;True;0;0;False;0;False;0.6352941;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;29;-1989.956,295.8886;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;42;-2000.701,127.0752;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;30;-1766.607,447.6382;Inherit;False;Property;_ManualControl;Manual Control;7;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;6;-1824.462,147.2135;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1894.802,-89.2038;Inherit;True;Property;_Gradient;Gradient;2;0;Create;True;0;0;False;0;False;-1;d410c104bd1ab4346b561aaf907d598d;d410c104bd1ab4346b561aaf907d598d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;36;-2172.382,1007.113;Inherit;False;Constant;_NoiseSpeed;Noise Speed;9;0;Create;True;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-2142.382,807.1122;Inherit;False;0;32;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;-1514.496,-13.81496;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;43;-1544.521,259.9594;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;34;-1921.383,943.1124;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1304.998,376.9254;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;-1659.382,915.1124;Inherit;True;Property;_TextureSample1;Texture Sample 1;8;0;Create;True;0;0;False;0;False;-1;None;351d5cc94f4b8428ab00b0708e3a9a69;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1272.58,179.5331;Inherit;False;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-1348.195,937.9301;Inherit;False;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-961.467,154.771;Inherit;True;23;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-962.3184,394.4104;Inherit;True;37;Noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-692.3184,277.4104;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;21;-988.0381,774.2372;Inherit;False;1821.773;607.9495;Edge Color;12;52;50;47;45;46;49;11;9;8;10;19;26;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;19;-938.0381,830.2615;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;8;-708.5053,828.4364;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-754.9704,1099.313;Inherit;False;Property;_EdgeRange;Edge Range;5;0;Create;True;0;0;False;0;False;0.09411766;0.6;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;9;-409.4542,828.535;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-169.0143,827.2494;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;49;25.05664,826.1534;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;50;284.0566,975.1534;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;458.9494,972.8265;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;45;152.8795,1158.166;Inherit;True;Property;_RampTex;Ramp Tex;10;0;Create;True;0;0;False;0;False;-1;None;6ff54184034074e42a0a12b394d8e355;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;52;344.0566,819.1534;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0.4;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;560.6119,1155.56;Inherit;True;RampColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-724.8649,-316.8912;Inherit;False;Property;_EdgeIntensity;Edge Intensity;6;0;Create;True;0;0;False;0;False;1;25.23;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-783.174,-532.514;Inherit;True;47;RampColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-987.0002,-121.4;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;6546508d5cac64232af9713e8fb5acb8;6546508d5cac64232af9713e8fb5acb8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;623.3669,816.5378;Inherit;True;EdgeColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-396.7641,17.97089;Inherit;False;26;EdgeColor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-487.1857,-345.7902;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;13;-1128.767,-671.3707;Inherit;False;Property;_EdgeColor;Edge Color;4;0;Create;True;0;0;False;0;False;0,0,0,0;0.6886792,0.3972506,0.06821822,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;12;-195.1857,-115.7902;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-445.3709,186.9515;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Dissolve Easy Double;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;0
WireConnection;42;0;41;0
WireConnection;30;1;29;0
WireConnection;30;0;4;0
WireConnection;6;0;30;0
WireConnection;6;3;42;0
WireConnection;5;0;2;1
WireConnection;5;1;6;0
WireConnection;43;0;5;0
WireConnection;43;1;41;0
WireConnection;34;0;33;0
WireConnection;34;2;36;0
WireConnection;44;0;43;0
WireConnection;32;1;34;0
WireConnection;23;0;44;0
WireConnection;37;0;32;1
WireConnection;39;0;25;0
WireConnection;39;1;40;0
WireConnection;19;0;39;0
WireConnection;8;0;19;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;11;0;9;0
WireConnection;49;0;11;0
WireConnection;50;0;49;0
WireConnection;46;0;50;0
WireConnection;45;1;46;0
WireConnection;52;0;49;0
WireConnection;52;3;49;0
WireConnection;47;0;45;0
WireConnection;26;0;52;0
WireConnection;15;0;48;0
WireConnection;15;1;14;0
WireConnection;15;2;1;0
WireConnection;12;0;1;0
WireConnection;12;1;15;0
WireConnection;12;2;27;0
WireConnection;3;0;1;4
WireConnection;3;1;39;0
WireConnection;0;2;12;0
WireConnection;0;10;3;0
ASEEND*/
//CHKSM=7D91C627082C523A7EE6D9C14A1E52D7CF934E2C