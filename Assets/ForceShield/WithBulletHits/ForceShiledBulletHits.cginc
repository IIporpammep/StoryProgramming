#ifndef FORCE_SHIELD_BULLET_HITS
#define FORCE_SHIELD_BULLET_HITS

int _HitsCount = 0;
float _HitsRadius[10];
float3 _HitsObjectPosition[10];
float _HitsIntensity[10];

float DrawRing(float intensity, float radius, float dist)
{
    float border = 0.05;
    float currentRadius = lerp(0, radius, 1 - intensity);//expand radius over time 
    return intensity * (1 - smoothstep(currentRadius, currentRadius + border, dist) - (1 - smoothstep(currentRadius - border, currentRadius, dist)));
}

void CalculateHitsFactor_float(float3 objectPosition, out float factor)
{
    factor = 0;
    for (int i = 0; i < _HitsCount; i++)
    {
        float distanceToHit = distance(objectPosition, _HitsObjectPosition[i]);
        factor += DrawRing(_HitsIntensity[i], _HitsRadius[i], distanceToHit);
    }
    factor = saturate(factor);
}
#endif
