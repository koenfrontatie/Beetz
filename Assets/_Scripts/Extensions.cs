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

        // Get array of enum values
        T[] values = (T[])Enum.GetValues(typeof(T));

        // Find current index
        int currentIndex = Array.IndexOf(values, current);

        // Calculate next index
        int nextIndex = (currentIndex + 1) % values.Length;

        return values[nextIndex];
    }

    public static bool Includes(this LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) > 0;
    }
}
