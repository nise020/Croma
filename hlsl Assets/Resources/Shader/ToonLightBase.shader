Pass
{
    Name "UniversalForward"
    Tags { "LightMode" = "UniversalForward" }

    HLSLPROGRAM
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile _ _SHADOWS_SOFT
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
    #pragma multi_compile_fog
    #pragma multi_compile_instancing

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

    TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
    TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

    CBUFFER_START(UnityPerMaterial)
        float4 _BaseColor;
        float _ShadowStep;
        float _ShadowStepSmooth;
        float _SpecularStep;
        float _SpecularStepSmooth;
        float4 _SpecularColor;
        float _RimStepSmooth;
        float _RimStep;
        float4 _RimColor;
        float4 _EmissionColor;
        float _Metallic;
        float _Smoothness;
        float _GrayscaleAmount;
        float4 _AmbientColor; // 추가: 환경광 컬러
        float _CellEdgeWidth; // 추가: 셀 경계 강조 두께
        float4 _CellEdgeColor; // 추가: 셀 경계 강조 색상
    CBUFFER_END

    struct Attributes
    {
        float4 positionOS   : POSITION;
        float3 normalOS     : NORMAL;
        float4 tangentOS    : TANGENT;
        float2 uv           : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float2 uv            : TEXCOORD0;
        float4 normalWS      : TEXCOORD1;
        float4 tangentWS     : TEXCOORD2;
        float4 bitangentWS   : TEXCOORD3;
        float3 viewDirWS     : TEXCOORD4;
        float4 shadowCoord   : TEXCOORD5;
        float4 fogCoord      : TEXCOORD6;
        float3 positionWS    : TEXCOORD7;
        float4 positionCS    : SV_POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    Varyings vert(Attributes input)
    {
        Varyings output = (Varyings)0;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);

        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
        float3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;

        output.positionCS = vertexInput.positionCS;
        output.positionWS = vertexInput.positionWS;
        output.uv = input.uv;
        output.normalWS = float4(normalInput.normalWS, viewDirWS.x);
        output.tangentWS = float4(normalInput.tangentWS, viewDirWS.y);
        output.bitangentWS = float4(normalInput.bitangentWS, viewDirWS.z);
        output.viewDirWS = viewDirWS;
        output.fogCoord = ComputeFogFactor(output.positionCS.z);
        return output;
    }

    float4 frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_INSTANCE_ID(input);

        float2 uv = input.uv;
        float3 N = normalize(input.normalWS.xyz);
        float3 V = normalize(input.viewDirWS.xyz);

        // 메인 라이트 정보
        Light mainLight = GetMainLight();
        float3 L = normalize(mainLight.direction);
        float3 H = normalize(V + L);

        float NV = saturate(dot(N, V));
        float NL = saturate(dot(N, L));
        float NH = saturate(dot(N, H));

        // Toon 단계화
        float shadowStep = step(_ShadowStep, NL);
        float shadowSmooth = smoothstep(_ShadowStep - _ShadowStepSmooth, _ShadowStep + _ShadowStepSmooth, NL);
        float shadow = lerp(0, 1, shadowSmooth);

        // 스펙큘러 단계화
        float specStep = step(_SpecularStep, NH);
        float specSmooth = smoothstep(_SpecularStep - _SpecularStepSmooth, _SpecularStep + _SpecularStepSmooth, NH);
        float specular = lerp(0, 1, specSmooth);

        // 림 라이트
        float rimDot = 1.0 - NV;
        float rim = smoothstep(_RimStep - _RimStepSmooth, _RimStep + _RimStepSmooth, rimDot);

        // 텍스처 및 컬러
        float4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        float3 baseColor = baseMap.rgb * _BaseColor.rgb;
        float gray = dot(baseColor, float3(0.299, 0.587, 0.114));
        baseColor = lerp(baseColor, float3(gray, gray, gray), _GrayscaleAmount);

        // 그림자 적용
        input.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
        float shadowAtten = MainLightRealtimeShadow(input.shadowCoord);

        // 환경광
        float3 ambient = _AmbientColor.rgb * baseColor;

        // Toon Diffuse
        float3 diffuse = mainLight.color.rgb * baseColor * shadow * shadowAtten;

        // Toon Specular
        float3 specColor = _SpecularColor.rgb * specular * shadow * shadowAtten * _Smoothness;

        // Rim
        float3 rimColor = _RimColor.rgb * rim;

        // Emission
        float4 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);
        float3 emission = emissionMap.rgb * _EmissionColor.rgb;

        // 셀 경계 강조 (옵션)
        float edge = abs(NL - _ShadowStep);
        float edgeMask = smoothstep(0, _CellEdgeWidth, edge);
        float3 cellEdge = lerp(_CellEdgeColor.rgb, float3(0,0,0), edgeMask);

        // 최종 컬러 합산
        float3 finalColor = diffuse + ambient + specColor + rimColor + emission + cellEdge;
        finalColor = MixFog(finalColor, input.fogCoord);

        return float4(finalColor, 1.0);
    }
    ENDHLSL
}

// ... (Outline Pass, ShadowCaster Pass 등 기존 코드 동일)