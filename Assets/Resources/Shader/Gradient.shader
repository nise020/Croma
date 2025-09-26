Shader "Custom/Gradient"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _TopAlpha ("Top Alpha", Range(0,2)) = 1
        _BottomAlpha ("Bottom Alpha", Range(0,1)) = 0
        _GradientPower ("Gradient Power", Range(0.1, 5)) = 1
        _GradientDirection ("Gradient Direction", Vector) = (1, 0, 0, 0) // 추가됨
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float _TopAlpha;
            float _BottomAlpha;
            float _GradientPower;
            float4 _GradientDirection; // 추가됨

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float Dither(float2 uv)
            {
                int x = int(fmod(uv.x * _ScreenParams.x, 4));
                int y = int(fmod(uv.y * _ScreenParams.y, 4));
                float threshold = ((x + y * 4) + 0.5) / 16.0;
                return threshold;
            }
            
           fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
            
                float2 dir = normalize(_GradientDirection.xy);
                float2 centeredUV = i.uv - 0.5;
            
                float t = dot(centeredUV, dir) + 0.5; // 다시 0~1 범위로 보정
                t = saturate(t); // clamp between 0 and 1
                t = pow(t, _GradientPower); // apply gradient power
            
                float gradientAlpha = lerp(_TopAlpha, _BottomAlpha, t);
            
                float noise = Dither(i.uv);
                gradientAlpha = saturate(gradientAlpha + (noise - 0.5) / 255.0);
            
                fixed4 finalColor = _MainColor * tex;
                finalColor.a *= gradientAlpha;
                return finalColor;
            }

            ENDCG
        }
    }
}
