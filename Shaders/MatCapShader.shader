Shader "Unlit/MatCapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Matcap ("Matcap Texture", 2D) = "white" {}
        _MatcapIntensity("Matcap Intensity", Float) = 1.0
        _RampTex ("Ramp Texture", 2D) = "white" {}
        _MatcapAdd ("Matcap Add Texture", 2D) = "white" {}
        _MatcapAddIntensity("Matcap Add Intensity", Float) = 1.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 world_normal : TEXCOORD1;
                float3 world_pos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Matcap;
            float _MatcapIntensity;
            sampler2D _RampTex;
            sampler2D _MatcapAdd;
            float _MatcapAddIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.world_normal = mul(float4(v.normal, 0.0), unity_WorldToObject);
                o.world_pos = mul(unity_WorldToObject, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 diffuseCol = tex2D(_MainTex, i.uv);

                // base matcap
                float3 normal_world = normalize(i.world_normal);
                float3 normal_viewspace = mul(UNITY_MATRIX_V, float4(normal_world, 0.0)).xyz;
                float2 uv_matcap = (normal_viewspace.xy + float2(1.0, 1.0)) * .5;
                fixed4 matcapCol = tex2D(_Matcap, uv_matcap) * _MatcapIntensity;

                // ramp
                half3 world_view = normalize(_WorldSpaceCameraPos.xyz - i.world_pos);
                half NdotV = saturate(dot(world_view, normal_world));
                half fresnel = 1.0 - NdotV;
                half2 uv_ramp = half2(fresnel, .5);
                fixed4 ramp_color = tex2D(_RampTex, uv_ramp);

                // add matcap
                fixed4 matcapAddCol = tex2D(_MatcapAdd, uv_matcap) * _MatcapAddIntensity;

                // combine
                fixed4 finalCol = diffuseCol * matcapCol * ramp_color + matcapAddCol;
                return finalCol;
                
            }
            ENDCG
        }
    } 
}
