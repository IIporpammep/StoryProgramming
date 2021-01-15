Shader "Unlit/RayMarchingBasic"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Off
        ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            #define MAX_STEPS 200
            #define MAX_DIST 200
            #define SURF_DIST 0.001

            uniform float4x4 _FrustumCornersVS;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 ray : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                int index = v.uv.z;
                o.ray = _FrustumCornersVS[index].xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                return o;
            }

            float GetDistance(float3 position)
            {
                float distanceToSphere = length(position - float3(0, 1, 0)) - 0.5;
                float distanceToPlane = position.y;
                return min(distanceToSphere, distanceToPlane);
            }

            float RayMarch(float3 rayOrigin, float3 rayDirection)
            {
                float distanceFromOrigin = 0;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 position = rayOrigin + distanceFromOrigin * rayDirection;
                    float distanceToSurface = GetDistance(position);
                    distanceFromOrigin += distanceToSurface;
                    if (distanceToSurface < SURF_DIST || distanceFromOrigin > MAX_DIST)
                    {
                        break;
                    }
                }
                return distanceFromOrigin;
            }

            float3 GetNormal(float3 position)
            {
                float distanceToSurface = GetDistance(position);
                float2 offsets = float2(0.0001, 0);
                float3 normal = distanceToSurface - float3(GetDistance(position - offsets.xyy),
                                                           GetDistance(position - offsets.yxy),
                                                           GetDistance(position - offsets.yyx));

                return normalize(normal);
            }

            float3 GetLight(float3 position)
            {
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightPosition = position + lightDirection * 100;
                float3 surfaceNormal = GetNormal(position);

                float3 diffuseLighting = saturate(dot(surfaceNormal, lightDirection)) * _LightColor0;

                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - position.xyz);
                float shiniess = 50;

                float cosAlpha = dot(reflect(-lightDirection, surfaceNormal), viewDirection);
                float3 specularLighting = pow(max(0, cosAlpha), shiniess) * _LightColor0;

                float3 resultLight = diffuseLighting + specularLighting; // diffuseLighting + specular;

                // Start not from the point, but a little bit above it in the normal direction, otherwise RayMarching will exit imminently
                // and everything will be in the shadows.
                float distanceToLight = RayMarch(position + surfaceNormal * SURF_DIST * 2, lightDirection);
                if (distanceToLight < length(lightPosition - position))
                {
                    //In shadow.
                    resultLight *= 0.1;
                }
                return resultLight;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float distance = RayMarch(_WorldSpaceCameraPos, normalize(i.ray));

                float3 intersectionPoint = _WorldSpaceCameraPos + normalize(i.ray) * distance;

                float3 color = GetLight(intersectionPoint);

                return float4(color, 1);
            }
            ENDCG
        }
    }
}