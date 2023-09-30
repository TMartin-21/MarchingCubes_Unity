// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lit/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude("Amplitude", Float) = 1
        _Wavelength("Wavelength", Float) = 1
        _Speed("Speed", Float) = 1
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
                float4 pos : POSITION;
                fixed4 normal : NORMAL;
            };

            struct v2f
            {
                half3 objNormal : NORMAL;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Amplitude;
            float _Wavelength;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;

                float w = 2.0f / _Wavelength;
                float f = _Speed * w;
                
                fixed3 dir = fixed3(-1.2, 0, -8.23);
                v.pos.y = _Amplitude * sin((v.pos.x * dir.x + v.pos.z * dir.z) * w + _Time * f);
                
                float dx = _Amplitude * w * dir.x * cos((v.pos.x * dir.x + v.pos.z * dir.z) * w + _Time * f);
                float dy = _Amplitude * w * dir.z * cos((v.pos.x * dir.x + v.pos.z * dir.z) * w + _Time * f);

                fixed3 binormal = fixed3(1, dx, 0);
                fixed3 tangent = fixed3(0, dy, 1);

                fixed3 normal = normalize(cross(binormal, tangent));

                o.objNormal = normal;
                o.pos = UnityObjectToClipPos(v.pos);
                return o;
            }

            CBUFFER_START(_CustomLight)
                float3 _DirectionalLightColor;
                float3 _DirectionalLightDirection;
            CBUFFER_END

            float3 shade(
                half3 normal, 
                half3 lightDir, 
                half3 viewDir,
                fixed3 powerDensity,
                fixed3 materialColor,
                fixed3 specularColor,
                float shininess)
            {
                
                

                return float3(0.0f, 0.0f, 0.0f);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 base;
                base.rgb = i.objNormal;
                return fixed4(0, 0.5, 0.5, 0);
            }
            ENDCG
        }
    }
}
