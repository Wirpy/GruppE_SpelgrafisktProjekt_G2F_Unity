Shader "FullScreen/Kuwahara_shader"
{
    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch switch2

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    // The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
    // struct PositionInputs
    // {
    //     float3 positionWS;  // World space position (could be camera-relative)
    //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
    //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
    //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
    //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
    //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
    // };

    // To sample custom buffers, you have access to these functions:
    // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
    // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
    // float4 CustomPassSampleCustomColor(float2 uv);
    // float4 CustomPassLoadCustomColor(uint2 pixelCoords);
    // float LoadCustomDepth(uint2 pixelCoords);
    // float SampleCustomDepth(float2 uv);

    // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
    // you can check them out in the source code of the core SRP package.
    float4 FullSreenPass(Varyings varyings) : SV_Target
    {
        int RADIUS = 3;

        float3 mean[4] = {
            float3(0, 0, 0),
            float3(0, 0, 0),
            float3(0, 0, 0),
            float3(0, 0, 0)
        };

        float3 sigma[4] = {
            float3(0, 0, 0),
            float3(0, 0, 0),
            float3(0, 0, 0),
            float3(0, 0, 0)
        };

        float2 offsets[4] = {
            float2(-3, -3),
            float2(-3, 0),
            float2(0, -3),
            float2(0, 0)
        };    

        float2 pos;
        float3 col = float3(0, 0, 0);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j <= RADIUS; j++)
            {
                for (int k = 0; k <= RADIUS; k++)
                {
                    pos = float2(j, k) + offsets[i];
                    float2 samplePosition = varyings.positionCS.xy + pos;
                    col = CustomPassLoadCameraColor(samplePosition, 0);
                    
                    mean[i] += col;
                    sigma[i] += col * col;
                }
            }
        }
        
        float n = pow(RADIUS + 1, 2);
        float sigma_f;

        float min = 1e10;

        for (int i = 0; i < 4; i++)
        {
            mean[i] /= n;
                sigma[i] = abs(sigma[i] / n - mean[i] * mean[i]);
                sigma_f = sigma[i].r + sigma[i].g + sigma[i].b;
    
            if (sigma_f < min)
            {
                min = sigma_f;
                col = mean[i];
            }
        }

        return float4(col, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Custom Pass 0"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
