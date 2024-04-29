using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SampleObjectsSO", menuName = "Collections/Sample Objects", order = 1)]
public class SampleObjectsSO : ScriptableObject
{
    public List<SampleObject> Collection = new List<SampleObject>();
}

[CreateAssetMenu(fileName = "GameObjectsSO", menuName = "Collections/Game Objects", order = 1)]
public class GameObjectsSO : ScriptableObject
{
    public List<GameObject> Collection = new List<GameObject>();
}

[CreateAssetMenu(fileName = "TexturesSO", menuName = "Collections/Textures", order = 1)]
public class Texture2DSO : ScriptableObject
{
    public List<Texture2D> Collection = new List<Texture2D>();
}

[CreateAssetMenu(fileName = "SampleObjectSO", menuName = "SampleObject", order = 1)]
public class SampleObjectSO : ScriptableObject
{
    public string ID;
    public string Name;
    public int Template;
    
    public SampleObject SampleObject;
    public Texture2D Icon;
}


