using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        foreach (Transform child in t)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static void DestroyChildrenImmediate(this Transform t)
    {
        foreach (Transform child in t)
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
    }




    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        Debug.Log(GetPath(filename));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(filename), content);
    }

    public static List<T> ReadListFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;

    }

    public static T ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);

        return res;

    }

    private static string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }

    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}