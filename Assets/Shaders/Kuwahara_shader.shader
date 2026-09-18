Shader "FullScreen/Kuwahara_shader"
{
    Properties{ _Radius ("Radius", Int) = 3 }

    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch switch2

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
    
    int _Radius;
    
    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
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
            float2(-_Radius, -_Radius),
            float2(-_Radius, 0),
            float2(0, -_Radius),
            float2(0, 0)
        };    

        float2 pos;
        float3 col = float3(0, 0, 0);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j <= _Radius; j++)
            {
                for (int k = 0; k <= _Radius; k++)
                {
                    pos = float2(j, k) + offsets[i];
                    float2 samplePosition = varyings.positionCS.xy + pos;
                    col = CustomPassLoadCameraColor(samplePosition, 0);
                    
                    mean[i] += col;
                    sigma[i] += col * col;
                }
            }
        }
        
        float n = pow(_Radius + 1, 2);
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
