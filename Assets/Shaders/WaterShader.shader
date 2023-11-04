// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lit/WaterShader"
{
    Properties
    {
        _WaterColor("WaterColor", COLOR) = (1, 1, 1, 1)
        _Amplitude("Amplitude", Float) = 1
        _Wavelength("Wavelength", Float) = 1
        _Speed("Speed", Float) = 1
        _WaveCount("WaveCount", Int) = 0

        _HurstExponent("HurstExponent", Float) = 0
        _FrequencyMultiplier("FrequencyMultiplier", Float) = 0
        _PhaseMultiplier("PhaseMultiplier", Float) = 0
        
        _Specular("Specular", Range(0.0, 1.0)) = 1
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 1
        _Cube("Cubemap", CUBE)=""{}
    }
    SubShader
    {
        Tags 
        { 
            "LightMode"="UniversalForward"
            "RenderType"="Opaque" 
        }
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                fixed3 normal : TEXCOORD2;
            };

            float _Amplitude;
            float _Wavelength;
            float _Speed;
            int _WaveCount;
            int _NumberOfWaves;
            float _HurstExponent;
            float _FrequencyMultiplier;
            float _PhaseMultiplier;
            
            static float PI = 3.14159265359;

            float3 getWave(float2 pos, float2 dir, float freq, float amp, float time)
            {
                float x = freq * dot(pos, dir) + time;
                float H = exp(sin(x) - 1.0);
                float dx = H * cos(x) * amp * dir.x * freq;
                float dz = H * cos(x) * amp * dir.y * freq;
                return float3(dx, H, dz);
            }

            float3 fbm(float3 pos)
            {
                float height = 0;
                float amplitude = _Amplitude;
                float frequency = 2.0 / _Wavelength;
                float phase = _Speed * frequency;
                float dx = 0;
                float dz = 0;
                float gain = exp2(-_HurstExponent);
                float maxValue = 0;
                float theta = 0;

                for (int i = 0; i < _WaveCount; i++)
                {
                    float2 dir = normalize(float2(cos(theta), sin(theta)));
                    float3 wave = getWave(pos.xz, dir, frequency, amplitude, _Time * phase);
                    
                    height += wave.y * amplitude;
                    dx += wave.x;
                    dz += wave.z;
                    maxValue += amplitude;
                    
                    frequency *= _FrequencyMultiplier;
                    amplitude *= gain;
                    phase *= _PhaseMultiplier;

                    theta += 360 / 45.0;
                }

                height = height / maxValue;

                return float3(dx, height, dz);
            }

            v2f vert (appdata_base v)
            {
                v2f o;

                float3 wave = fbm(v.vertex.xyz);
                float dx = wave.x;
                float dz = wave.z;
                v.vertex.y += wave.y;

                fixed3 binormal = fixed3(1, dx, 0);
                fixed3 tangent = fixed3(0, dz, 1);
                fixed3 normal = cross(tangent, binormal);
                o.normal = normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 _WaterColor;
            float _Specular;
            float _Smoothness;
            samplerCUBE _Cube;
       
            fixed4 diffuseAmbient(fixed3 worldNormal)
            {
                //fixed4 _Color = fixed4(0.033529411764705882, 0.25882352941176473, 0.58098039215686275, 0);
                half costheta = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 ambient = ShadeSH9(half4(worldNormal, 1));
                fixed4 diffuse = costheta * _LightColor0;
                diffuse.rgb += ambient;
                diffuse.rgb *= _WaterColor.rgb;
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
            
            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = fixed4(1, 1, 1, 0);
                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal);
                fixed4 diff = diffuseAmbient(worldNormal);
                fixed4 spec = specular(worldNormal, i.worldPos);
                col *= diff + spec;
                return col;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
