Shader "Lit/TerrainShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color0("Color0", COLOR) = (1, 1, 1, 1)
        _Color1("Color1", COLOR) = (1, 1, 1, 1)
        _Color2("Color2", COLOR) = (1, 1, 1, 1)

        _Specular("Specular", Range(0.0, 1.0)) = 1
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1
    }
    SubShader
    {
        Tags { 
            "LightMode"="UniversalForward"  
            "RenderType"="Opaque" 
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"                // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc"    // for _LightColor0
            #include "AutoLight.cginc"              // shadow helper functions and macros

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                fixed3 normal : TEXCOORD1;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;        
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 _Color0;
            fixed4 _Color1;
            fixed4 _Color2;
            float _Specular;
            float _Smoothness;
       
            fixed4 diffuseAmbient(fixed3 worldNormal)
            {
                half costheta = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 ambient = ShadeSH9(half4(worldNormal, 1));
                fixed4 diffuse = costheta * _LightColor0;
                diffuse.rgb += ambient;
                //diffuse.rgb *= _Color0.rgb;
                return diffuse;
            }

            fixed4 specular(fixed3 worldNormal, float3 worldPos)
            {
                fixed3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                fixed3 halfway = normalize(viewDir + _WorldSpaceLightPos0.xyz);
                float cosdelta = max(0.0, dot(halfway, worldNormal));
                fixed4 spec = _LightColor0 * pow(cosdelta, _Specular * 128.0) * _Smoothness * fixed4(.7, .7, .7, 0);
                return spec;
            }
            // 447841 green
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1, 1, 1, 0);

                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal);
                fixed4 diff = diffuseAmbient(worldNormal);
                fixed4 spec = specular(worldNormal, i.worldPos);
                

                fixed3 up = fixed3(0, 1, 0);
                float cos_delta = dot(up, worldNormal);
                fixed3 terrain = lerp(_Color0, _Color1, cos_delta);
                diff.rgb *= terrain;

                col *= diff + spec;
                return col;               
            }
            ENDCG
        }
    }
}
