using System;
using UnityEngine;

public static class Extensions
{
    public static T NextEnumValue<T>(this T current) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        T[] values = (T[])Enum.GetValues(typeof(T));

        int currentIndex = Array.IndexOf(values, current);

        int nextIndex = (currentIndex + 1) % values.Length;

        return values[nextIndex];
    }
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static bool Includes(this LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) > 0;
    }

    public static void DestroyChildren(this Transform t)
    {
        foreach(Transform child in t)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }
}
