Shader "Unlit/CustomShader"
{
    Properties
    {
        _MainTex ("Texture 0", 2D) = "white" {}
        _Tiling ("Tiling 0", Float) = 1.0
        _OcclusionMap("Occlusion 0", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "white" {}
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

            struct v2f
            {
                half3 objNormal : TEXCOORD0;
                float3 coords : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 pos : SV_POSITION;
            };

            float _Tiling;

            v2f vert (float4 pos : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(pos);
                o.coords = pos.xyz * _Tiling;
                o.objNormal = normal;
                o.uv = uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _OcclusionMap;
            sampler2D _BumpMap;

            fixed4 frag(v2f i) : SV_Target
            {
                half3 blend = abs(i.objNormal);
                blend /= dot(blend, 1.0);

                fixed4 cx = tex2D(_MainTex, i.coords.yz);
                fixed4 cy = tex2D(_MainTex, i.coords.xz);
                fixed4 cz = tex2D(_MainTex, i.coords.xy);

                // bledn texture based on weights
                fixed4 c = cx * blend.x + cy * blend.y * cz * blend.z;
    
                c *= tex2D(_OcclusionMap, i.uv);
                return c;
            }
            ENDCG
        }
    }
}
