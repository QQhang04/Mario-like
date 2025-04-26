Shader "Unlit/PassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RimMin ("RimMin", Range(-1, 1)) = 0.0
        _RimMax ("RimMax", Range(0, 2)) = 0.0
        
        _TextureEmiss ("TextureEmiss", Float) = 0.0
        
        _InnerColor ("Inner Color", Color) = (0,0,0,0)
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimIntensiity ("RimIntensiity", Float) = 0.0
        
        _FlowTilling ("Flow Tilling", Vector) = (1,1,0,0)
        _FlowSpeed ("Flow Speed", Vector) = (1,1,0,0)
        _TextureFlow ("Texture Flow", 2D) = "white" {}
        _FlowIntensity("Flow Intensity", Float) = 0.5
        _FlowColor ("Flow Color", Color) = (1,1,1,1)
        
        _InnerAlpha ("Inner Alpha", Range(0.0, 1.0)) = 0.0
        
    }
    SubShader
    {
        Tags {"Queue" = "Transparent"}
        Pass
        {
            ZWrite On
            ColorMask 0
        }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 pos_world : TEXCOORD1;
                float3 normal_world : TEXCOORD2;

                float3 pivot_world : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RimMin;
            float _RimMax;
            float _TextureEmiss;
            fixed4 _InnerColor;
            fixed4 _RimColor;
            float _RimIntensiity;
            float4 _FlowTilling;
            float4 _FlowSpeed;
            sampler2D _TextureFlow;
            float _FlowIntensity;
            float _InnerAlpha;
            fixed4 _FlowColor;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 normalWorld = UnityObjectToWorldNormal(v.normal);
                float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal_world = normalWorld;
                o.pos_world = posWorld;
                o.uv = v.texcoord;

                //3维补4维时 点的话后面补1 向量则补0
                o.pivot_world = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half3 normal_world = normalize(i.normal_world);
                half3 view_world = normalize(_WorldSpaceCameraPos.xyz - i.pos_world);
                // fixed4 col = tex2D(_MainTex, i.uv);
                half NdotV = saturate(dot(normal_world, view_world));
                half fresnel = 1.0 - NdotV;

                half emiss = tex2D(_MainTex, i.uv).b;
                emiss = pow(emiss, _TextureEmiss);

                // 边缘光强度
                half final_fresnel = saturate(fresnel + emiss);

                // 当 final_fresnel 较大（即视角与法线的夹角较大，通常是物体边缘），最终颜色将接近边缘光颜色（_RimColor）并乘上一个强度因子（_RimIntensiity），使得边缘光更加明亮。
                half3 final_rim_color = lerp(_InnerColor.xyz, _RimColor.xyz * _RimIntensiity, final_fresnel);
                half final_rim_alpha = final_fresnel;

                // uv扫描流光效果
                half2 uv_flow = (i.pos_world.xy - i.pivot_world.xy) * _FlowTilling;
                uv_flow += _Time.y * _FlowSpeed.xy;
                float4 flow_rgba = tex2D(_TextureFlow, uv_flow) * _FlowIntensity * _FlowColor;

                // 最后整合
                float3 final_col = final_rim_color + flow_rgba.xyz;
                float final_alpha = final_rim_alpha + _InnerAlpha;
                return fixed4(final_col, final_alpha);
            }
                
            ENDCG
        }
    }
}
