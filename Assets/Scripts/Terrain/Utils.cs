using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float FBM(float x,float y, int oct,float persistance)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maValue = 0;
        for (int i = 0; i < oct; i++)
        {
            total += Mathf.PerlinNoise(x  * frequency, y  * frequency) * amplitude;
            maValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }
        return total / maValue;
    }

    public static float Map(float value,float originalMin,float originalMax, float targetMin, float targetMax)
    {
        return (value - originalMin) * (targetMax - targetMin) / (originalMax - originalMin) + targetMin;
    }


}
