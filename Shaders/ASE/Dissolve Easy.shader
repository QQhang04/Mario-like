// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Dissolve Easy"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Gradient("Gradient", 2D) = "white" {}
		_ChangeAmount("Change Amount", Range( 0 , 1)) = 0.6352941
		_EdgeColor("Edge Color", Color) = (0,0,0,0)
		_EdgeRange("Edge Range", Range( 0 , 2)) = 0.09411766
		_EdgeIntensity("Edge Intensity", Float) = 1
		[Toggle(_MANUALCONTROL_ON)] _ManualControl("Manual Control", Float) = 0
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
		uniform float4 _EdgeColor;
		uniform float _EdgeIntensity;
		uniform sampler2D _Gradient;
		SamplerState sampler_Gradient;
		uniform float4 _Gradient_ST;
		uniform float _ChangeAmount;
		uniform float _EdgeRange;
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
			float Gradient23 = ( tex2D( _Gradient, uv_Gradient ).r - (-1.0 + (staticSwitch30 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float smoothstepResult19 = smoothstep( 0.0 , 1.0 , Gradient23);
			float smoothstepResult18 = smoothstep( 0.0 , 1.0 , ( 1.0 - ( distance( smoothstepResult19 , 0.5 ) / _EdgeRange ) ));
			float EdgeColor26 = smoothstepResult18;
			float4 lerpResult12 = lerp( tex2DNode1 , ( _EdgeColor * _EdgeIntensity * tex2DNode1 ) , EdgeColor26);
			o.Emission = lerpResult12.rgb;
			o.Alpha = 1;
			float smoothstepResult20 = smoothstep( 0.39 , 0.5 , Gradient23);
			clip( ( tex2DNode1.a * smoothstepResult20 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
1;96;1209;646;2399.761;89.11188;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;22;-2179.181,-139.2038;Inherit;False;1089.899;804.7397;Gradient;8;23;5;2;6;4;28;29;30;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;28;-2156.669,298.0261;Inherit;False;1;0;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;29;-1989.956,295.8886;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2083.229,534.7965;Inherit;False;Property;_ChangeAmount;Change Amount;3;0;Create;True;0;0;False;0;False;0.6352941;0.44;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;30;-1766.607,447.6382;Inherit;False;Property;_ManualControl;Manual Control;7;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1894.802,-89.2038;Inherit;True;Property;_Gradient;Gradient;2;0;Create;True;0;0;False;0;False;-1;d410c104bd1ab4346b561aaf907d598d;6e9e3841a0552a34cb7c38b3628da853;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;6;-1824.462,147.2135;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;-1514.496,-13.81496;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;21;-2201.811,740.7872;Inherit;False;1638.773;509.9495;Edge Color;8;24;18;11;9;10;8;19;26;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1474.819,253.856;Inherit;False;Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-2199.737,978.2318;Inherit;False;23;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;19;-2151.811,796.8114;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1968.743,1065.863;Inherit;False;Property;_EdgeRange;Edge Range;5;0;Create;True;0;0;False;0;False;0.09411766;0.05;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;8;-1922.278,794.9863;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;9;-1623.227,795.0849;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-1382.787,793.7993;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;18;-1170.712,790.7872;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-906.4055,791.4948;Inherit;False;EdgeColor;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-1044.467,274.771;Inherit;False;23;Gradient;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-987.0002,-121.4;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;6546508d5cac64232af9713e8fb5acb8;6546508d5cac64232af9713e8fb5acb8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-731.9016,-433.5057;Inherit;False;Property;_EdgeColor;Edge Color;4;0;Create;True;0;0;False;0;False;0,0,0,0;0.2735849,0.153741,0.01677644,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-815.8649,-256.8912;Inherit;False;Property;_EdgeIntensity;Edge Intensity;6;0;Create;True;0;0;False;0;False;1;22.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;20;-793.5028,278.2819;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.39;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-403.7641,-9.029107;Inherit;False;26;EdgeColor;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-492.1857,-333.7902;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;12;-195.1857,-115.7902;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-445.3709,186.9515;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Dissolve Easy;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;0
WireConnection;30;1;29;0
WireConnection;30;0;4;0
WireConnection;6;0;30;0
WireConnection;5;0;2;1
WireConnection;5;1;6;0
WireConnection;23;0;5;0
WireConnection;19;0;24;0
WireConnection;8;0;19;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;11;0;9;0
WireConnection;18;0;11;0
WireConnection;26;0;18;0
WireConnection;20;0;25;0
WireConnection;15;0;13;0
WireConnection;15;1;14;0
WireConnection;15;2;1;0
WireConnection;12;0;1;0
WireConnection;12;1;15;0
WireConnection;12;2;27;0
WireConnection;3;0;1;4
WireConnection;3;1;20;0
WireConnection;0;2;12;0
WireConnection;0;10;3;0
ASEEND*/
//CHKSM=2786A9DA1BF0E26DA2DA2FEE530EF804F711A1E7