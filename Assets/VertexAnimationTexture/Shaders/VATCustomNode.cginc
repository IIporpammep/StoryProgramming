#ifndef VAT_CUSTOM_NODE
#define VAT_CUSTOM_NODE

sampler2D _PositionsTex;
sampler2D _RotationsTex;
float _Frames;
float _State;
float3 _BoundsCenter;
float3 _BoundsExtents;
float3 _StartBoundsCenter;
float3 _StartBoundsExtents;


float3 DecodePositionInBounds(float3 encodedPosition, float3 boundsCenter, float3 boundsExtents)
{
    return boundsCenter + float3(lerp(-boundsExtents.x, boundsExtents.x, encodedPosition.x), lerp(-boundsExtents.y, boundsExtents.y, encodedPosition.y), lerp(-boundsExtents.z, boundsExtents.z, encodedPosition.z));
}

void CalculatePositionFromVAT_float(float3 inputObjectPosition, float4 vertexColor, out float3 objectPosition)
{
    float3 pivot = vertexColor.xyz;
    float3 decodedPivot = DecodePositionInBounds(pivot, _StartBoundsCenter, _StartBoundsExtents);

    float3 offset = inputObjectPosition - decodedPivot;


    float idOfMeshPart = vertexColor.a;
    float currentFrame = _State;
    float3 vatPosition = tex2Dlod(_PositionsTex, float4(idOfMeshPart, currentFrame, 0, 0)).xyz;

    objectPosition = offset + DecodePositionInBounds(vatPosition, _BoundsCenter, _BoundsExtents);
}
#endif
