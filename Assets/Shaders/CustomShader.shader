// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/CustomShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0
        _Color("Color", COLOR) = (0.5, 0.5, 0.5, 1)
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1
        _Specular("Specular", Range(0.0, 1.0)) = 1
        _Cubemap("Cubemap", CUBE)=""{}
    }
    SubShader
    {
        Pass
        {
            Tags {
                "LightMode"="UniversalForward"
                "IgnoreProjector"="True"  
                "RenderType"="Opaque"
            }
            Cull [_Culling]
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"                // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc"    // for _LightColor0
            #include "AutoLight.cginc"              // shadow helper functions and macros

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                fixed3 normal : TEXCOORD2;
            };

            fixed4 _Color;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;        
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            
            float _Smoothness;
            float _Specular;
            samplerCUBE _Cubemap;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1, 1, 1, 1);
                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal);  
                
                // Diffuse reflection
                half nl = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                fixed4 diff = nl * _LightColor0;
                diff.rgb += ShadeSH9(half4(worldNormal, 1));
                
                // Specular reflection
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                fixed3 halfway = normalize(viewDir + _WorldSpaceLightPos0.xyz);
                float cosdelta = max(0.0, dot(halfway, worldNormal));
                fixed4 spec = _LightColor0 * pow(cosdelta, _Specular * 128.0) * _Smoothness * fixed4(.7, .7, .7, 0);

                //float3 reflection = reflect(-viewDir, worldNormal);
                //fixed4 refl = texCUBE(_Cubemap, normalize(reflection));
                
                col *= diff * _Color + spec;
                return col;
            }
            ENDCG
        }
    }
}
