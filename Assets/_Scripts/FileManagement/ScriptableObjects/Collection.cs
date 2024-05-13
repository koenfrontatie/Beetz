using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "GameObjectsSO", menuName = "Collections/Game Objects", order = 2)]
public class GameObjectsSO : ScriptableObject
{
    public List<GameObject> Collection = new List<GameObject>();
}

[CreateAssetMenu(fileName = "TexturesSO", menuName = "Collections/Textures", order = 3)]
public class Texture2DSO : ScriptableObject
{
    public List<Texture2D> Collection = new List<Texture2D>();
}

[CreateAssetMenu(fileName = "SampleObjectSO", menuName = "SampleObject", order = 4)]
public class SampleObjectSO : ScriptableObject
{
    public string ID;
    public string Name;
    public int Template;
    
    public SampleObject SampleObject;
    public Texture2D Icon;
}


