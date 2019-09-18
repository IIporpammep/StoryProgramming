#ifndef VAT_CUSTOM_NODE
#define VAT_CUSTOM_NODE

sampler2D _PositionsTex;
sampler2D _PositionsTexB;
sampler2D _RotationsTex;
float _State;
int _PartsCount;
float3 _BoundsCenter;
float3 _BoundsExtents;
float3 _StartBoundsCenter;
float3 _StartBoundsExtents;
int _HighPrecisionMode;
int _PartsIdsInUV3;

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


//Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
//From UnityCG.cginc
inline float2 EncodeFloatRG(float v)
{
    float2 kEncodeMul = float2(1.0, 255.0);
    float kEncodeBit = 1.0 / 255.0;
    float2 enc = kEncodeMul * v;
    enc = frac(enc);
    enc.x -= enc.y * kEncodeBit;
    return enc;
}
inline float DecodeFloatRG(float2 enc)
{
    float2 kDecodeDot = float2(1.0, 1 / 255.0);
    return dot(enc, kDecodeDot);
}

float Remap(float In, float2 InMinMax, float2 OutMinMax)
{
    return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}


void CalculateVAT_float(float3 inputObjectPosition, float3 inputObjectNormal, float4 vertexColor, float2 uv3, out float3 objectPosition, out float3 rotatedNormal)
{
    float encodedPartId;
    if (_PartsIdsInUV3 == 1)
    {
        encodedPartId = Remap(DecodeFloatRG(uv3), float2(0, 1 - 1.0 / (float)_PartsCount), float2(0, 1)); //needs to be remapped to [0,1], because 1.0 will not be encoded properly using FloatRG encoding
    }
    else
    {
        encodedPartId = vertexColor.a;
    }

    //To prevent Bilinear FilterMode from interpolating between idOfMeshParts, sample over X axis must be in the centre of pixel.
    //Without this remap some parts of the mesh could be in wrong positions
    //something similar described there http://www.asawicki.info/news_1516_half-pixel_offset_in_directx_11.html
    float halfPixel = 1.0 / (_PartsCount * 2);
    float idOfMeshPart = Remap(encodedPartId, float2(0, 1), float2(halfPixel, 1 - halfPixel));

    float currentFrame = _State;
 
    float4 vatRotation = tex2Dlod(_RotationsTex, float4(idOfMeshPart, currentFrame, 0, 0));
    float4 decodedRotation = DecodeQuaternion(vatRotation);

    float3 pivot = vertexColor.xyz;
    float3 decodedPivot = DecodePositionInBounds(pivot, _StartBoundsCenter, _StartBoundsExtents);
    float3 offset = inputObjectPosition - decodedPivot;

    float3 rotated = RotateVectorUsingQuaternionFast(decodedRotation, offset);
    
    if (_HighPrecisionMode == 1)
    {
        float3 vatPosition = tex2Dlod(_PositionsTex, float4(idOfMeshPart, currentFrame, 0, 0)).xyz;
        float3 vatPositionB = tex2Dlod(_PositionsTexB, float4(idOfMeshPart, currentFrame, 0, 0)).xyz;
        float3 decodedPosition = float3(DecodeFloatRG(float2(vatPosition.x, vatPositionB.x)), DecodeFloatRG(float2(vatPosition.y, vatPositionB.y)), DecodeFloatRG(float2(vatPosition.z, vatPositionB.z)));
        objectPosition = rotated + DecodePositionInBounds(decodedPosition, _BoundsCenter, _BoundsExtents);
    }
    else
    {
        float3 vatPosition = tex2Dlod(_PositionsTex, float4(idOfMeshPart, currentFrame, 0, 0)).xyz;
        objectPosition = rotated + DecodePositionInBounds(vatPosition, _BoundsCenter, _BoundsExtents);
    }
    rotatedNormal = RotateVectorUsingQuaternionFast(decodedRotation, inputObjectNormal);
}
#endif
