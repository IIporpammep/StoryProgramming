Shader "Hidden/Custom/PostEffectGrid"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
        float _Blend;
		float _DistanceBtwLines;
		float _LinesWidth;
		float4 _LinesColor;

		///View space Reconstruction taken from ScalableAO.hlsl

		// Check if the camera is perspective.
        // (returns 1.0 when orthographic)
        float CheckPerspective(float x)
        {
            return lerp(x, 1.0, unity_OrthoParams.w);
        }
        // Reconstruct view-space position from UV and depth.
		//float3x3 proj = (float3x3)unity_CameraProjection;
        // p11_22 = (unity_CameraProjection._11, unity_CameraProjection._22)
        // p13_31 = (unity_CameraProjection._13, unity_CameraProjection._23)
        float3 ReconstructViewPos(float2 uv, float depth, float2 p11_22, float2 p13_31)
        {
            return float3((uv * 2.0 - 1.0 - p13_31) / p11_22 * CheckPerspective(depth), depth);
        }

	    // Boundary check for depth sampler
		// (returns a very large value if it lies out of bounds)
		float CheckBounds(float2 uv, float d)
		{
			float ob = any(uv < 0) + any(uv > 1);
		#if defined(UNITY_REVERSED_Z)
			ob += (d <= 0.00001);
		#else
			ob += (d >= 0.99999);
		#endif
			return ob * 1e8;
		}

		// Depth/normal sampling functions
        float SampleDepth(float2 uv)
        {
            float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv), 0));
            return d * _ProjectionParams.z + CheckBounds(uv, d);
        }

		float DrawLine(float position)
		{
		   float distance = frac( position * _DistanceBtwLines);
		   float lineCenter = 0.5;
		   float width = _LinesWidth/2;
		   return (1 - smoothstep(lineCenter,lineCenter + width, distance)) - (1 - smoothstep(lineCenter - width, lineCenter, distance));
		}

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

			// Parameters used in coordinate conversion
            float3x3 proj = (float3x3)unity_CameraProjection;
            float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
            float2 p13_31 = float2(unity_CameraProjection._13, unity_CameraProjection._23);

			float depth_o = SampleDepth(i.texcoord);
			float farPlane = _ProjectionParams.z;
			if(depth_o >= farPlane)
			{
			  return color;
			}

			// Reconstruct the view-space position.
            float3 vpos_o = ReconstructViewPos( i.texcoord, depth_o, p11_22, p13_31);

			//Reconstruct the world space
			//float4x4 viewTranspose = transpose(UNITY_MATRIX_V);
            //float3 worldNormal = mul(viewTranspose, float4(viewNormal.xyz, 0)).xyz; // 0 is used for directions, use 1 for positions
			float4x4 cameraToWorld = transpose(unity_WorldToCamera);
			float3 worldPos = mul(cameraToWorld, float4(vpos_o, 1)).xyz + _WorldSpaceCameraPos;

			float lineZ = DrawLine(worldPos.z);
			float lineX = DrawLine(worldPos.x);
		    float lineY = DrawLine(worldPos.y);
			float lineFactor = max( lineY, max(lineZ, lineX));

            return color + _LinesColor * lineFactor * _Blend;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}