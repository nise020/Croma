Shader "Custom/ToonShader_V2"
{
    Properties
    {
        _MainTex ("Albedo RGB", 2D) = "white" {}

        _BumpMap("Normal", 2D) = "Bump"{}
        _BumpPower("Normal Power", float) = 1

        [HDR]
        _RimCol("Rim Color", Color) = (1, 1, 1, 1)
        _RimPow("Rim Power", float) = 2

        [HDR]
        _SpecCol("Specular Color", Color) = (1, 1, 1, 1)
        _SpecPow("Specular Power", float) = 50

        [HDR]
        _FspecCol("Fake Specular Color", Color) = (1, 1, 1, 1)
        _FspecPow("Fake Specular Power", float) = 50

        [HDR]
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineRange("Outline Range", float) = 1

        _CeilPower("Ceil Power", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Opaque" = "Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        float4 _OutlineColor;
        float _OutlineRange;

        void vert (inout appdata_full v)
        {
            v.vertex.xyz = v.vertex.xyz + v.normal * _OutlineRange;
        }

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Emission = _OutlineColor;
        }
        ENDCG

        cull back
        CGPROGRAM
        #pragma surface surf Test
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;

        float4 _RimCol;
        float4 _SpecCol;
        float4 _FspecCol;

        float _BumpPower;
        float _RimPow;
        float _SpecPow;
        float _FspecPow;
        float _CeilPower;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 lightDir;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

            fixed3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            n = float3(n.r * _BumpPower, n.g * _BumpPower, n.b);
            o.Normal = normalize(n);

            float rim = saturate(pow(( 1- dot(o.Normal, IN.viewDir)), _RimPow));

            o.Emission = ((c.a * 0.5) + rim * _RimCol.rgb);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        float4 LightingTest (SurfaceOutput s, float3 lightDir, float viewDir, float atten)
        {
            float NdotL = saturate(dot(s.Normal, lightDir));

            NdotL = pow(saturate((ceil(NdotL * _CeilPower) / _CeilPower) * 0.5 + 0.5), 5);

            float3 h = normalize(viewDir + lightDir);
            float spec = saturate(dot(s.Normal, h));
            spec = saturate(pow(spec, _SpecPow));
            spec = ceil(spec) / 5;

            float4 finalColor;
            finalColor.rgb = s.Albedo * (NdotL + spec * _SpecCol.rgb);
            finalColor.a = s.Albedo;

            return finalColor;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
