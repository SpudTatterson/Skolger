Shader "Lpk/LightModel/ToonLightBase_WithYFade"
{
    Properties
    {
        _BaseMap            ("Texture", 2D)                       = "white" {}
        _BaseColor          ("Color", Color)                      = (0.5,0.5,0.5,1)
        _Transparency       ("Transparency", Range(0,1))          = 1   
        
        [Space]
        _ShadowStep         ("ShadowStep", Range(0, 1))           = 0.5
        _ShadowStepSmooth   ("ShadowStepSmooth", Range(0, 1))     = 0.04
        
        [Space] 
        _SpecularStep       ("SpecularStep", Range(0, 1))         = 0.6
        _SpecularStepSmooth ("SpecularStepSmooth", Range(0, 1))   = 0.05
        [HDR]_SpecularColor ("SpecularColor", Color)              = (1,1,1,1)
        
        [Space]
        _RimStep            ("RimStep", Range(0, 1))              = 0.65
        _RimStepSmooth      ("RimStepSmooth",Range(0,1))          = 0.4
        _RimColor           ("RimColor", Color)                   = (1,1,1,1)
        
        [Space]   
        _FadeRange          ("Fade Range", Float)                 = 1.0  // Distance over which the fade occurs
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Blend SrcAlpha OneMinusSrcAlpha  // Enable transparency blending
            //ZWrite Off                       // Disable depth writing for transparency
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

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Transparency;
                float _ShadowStep;
                float _ShadowStepSmooth;
                float _SpecularStep;
                float _SpecularStepSmooth;
                float4 _SpecularColor;
                float _RimStepSmooth;
                float _RimStep;
                float4 _RimColor;
                float _GlobalYClip;  // Global Y Clip property
                float _FadeRange;    // Range over which the fade occurs
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
                float4 normalWS      : TEXCOORD1;    // xyz: normal, w: viewDir.x
                float4 tangentWS     : TEXCOORD2;    // xyz: tangent, w: viewDir.y
                float4 bitangentWS   : TEXCOORD3;    // xyz: bitangent, w: viewDir.z
                float3 viewDirWS     : TEXCOORD4;
				float4 shadowCoord	 : TEXCOORD5;	// shadow receive 
				float4 fogCoord	     : TEXCOORD6;	
				float3 positionWS	 : TEXCOORD7;	
                float4 positionCS    : SV_POSITION;
                float visibilityFactor : TEXCOORD8;
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

                output.visibilityFactor = saturate((output.positionWS.y - _GlobalYClip) / _FadeRange);
                
                return output;
            }
            
            half remap(half x, half t1, half t2, half s1, half s2)
            {
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uv = input.uv;
                float3 N = normalize(input.normalWS.xyz);
                float3 V = normalize(input.viewDirWS.xyz);
                float3 L = normalize(_MainLightPosition.xyz);
                float3 H = normalize(V+L);
                
                float NV = dot(N,V);
                float NH = dot(N,H);
                float NL = dot(N,L);
                
                NL = NL * 0.5 + 0.5;

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                float specularNH = smoothstep((1-_SpecularStep * 0.05)  - _SpecularStepSmooth * 0.05, (1-_SpecularStep* 0.05)  + _SpecularStepSmooth * 0.05, NH);
                float shadowNL = smoothstep(_ShadowStep - _ShadowStepSmooth, _ShadowStep + _ShadowStepSmooth, NL);

                input.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                
                float shadow = MainLightRealtimeShadow(input.shadowCoord);
                
                float rim = smoothstep((1-_RimStep) - _RimStepSmooth * 0.5, (1-_RimStep) + _RimStepSmooth * 0.5, 0.5 - NV);
                
                float3 diffuse = _MainLightColor.rgb * baseMap * _BaseColor * shadowNL * shadow;
                
                float3 specular = _SpecularColor * shadow * shadowNL *  specularNH;
                
                float3 ambient =  rim * _RimColor + SampleSH(N) * _BaseColor * baseMap;

                // Calculate fade factor based on Y distance from the clip level
                float fadeFactor = input.visibilityFactor;
                
                // Calculate final alpha with fade
                float alpha = _Transparency - fadeFactor;
            
                float3 finalColor = diffuse + ambient + specular;
                finalColor = MixFog(finalColor, input.fogCoord);
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
}
