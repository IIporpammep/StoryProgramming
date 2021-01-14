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
                float distance = length(position) - 0.5;
                return distance;
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

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = 0;

                float distance = RayMarch(_WorldSpaceCameraPos, normalize(i.ray));

                if (distance < MAX_DIST)
                {
                    color.r = 1;
                }
                return color;
            }
            ENDCG
        }
    }
}