#ifndef VAT_CUSTOM_NODE
#define VAT_CUSTOM_NODE

sampler2D _PositionsTex;
sampler2D _RotationsTex;
float _State;
float3 _BoundsCenter;
float3 _BoundsExtents;
float3 _StartBoundsCenter;
float3 _StartBoundsExtents;


float3 DecodePositionInBounds(float3 encodedPosition, float3 boundsCenter, float3 boundsExtents)
{
    return boundsCenter + float3(lerp(-boundsExtents.x, boundsExtents.x, encodedPosition.x), lerp(-boundsExtents.y, boundsExtents.y, encodedPosition.y), lerp(-boundsExtents.z, boundsExtents.z, encodedPosition.z));
}

float4 DecodeQuaternion(float4 encodedRotation)
{
    return float4(lerp(-1, 1, encodedRotation.x), lerp(-1, 1, encodedRotation.y), lerp(-1, 1, encodedRotation.z), lerp(-1, 1, encodedRotation.w));
}

//The fast method of quaternion vector multiplication by Fabian Giesen (ryg of Farbrausch fame). Found in this blog post:
//https://blog.molecular-matters.com/2013/05/24/a-faster-quaternion-vector-multiplication/
float3 RotateVectorUsingQuaternionFast(float4 q, float3 v)
{
    float3 t = 2 * cross(q.xyz, v);
    return v + q.w * t + cross(q.xyz, t);
}

void CalculatePositionFromVAT_float(float3 inputObjectPosition, float4 vertexColor, float3 inputObjectNormal, out float3 objectPosition, out float3 rotatedNormal)
{
    float3 pivot = vertexColor.xyz;
    float3 decodedPivot = DecodePositionInBounds(pivot, _StartBoundsCenter, _StartBoundsExtents);

    float3 offset = inputObjectPosition - decodedPivot;

    float idOfMeshPart = vertexColor.a;
    float currentFrame = _State;
 
    float4 vatRotation = tex2Dlod(_RotationsTex, float4(idOfMeshPart, currentFrame, 0, 0));
    float4 decodedRotation = DecodeQuaternion(vatRotation);

    float3 rotated = RotateVectorUsingQuaternion(decodedRotation, offset);
       
    float3 vatPosition = tex2Dlod(_PositionsTex, float4(idOfMeshPart, currentFrame, 0, 0)).xyz;
    objectPosition = rotated + DecodePositionInBounds(vatPosition, _BoundsCenter, _BoundsExtents);

    rotatedNormal = RotateVectorUsingQuaternion(decodedRotation, inputObjectNormal);
}
#endif
