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

//[CreateAssetMenu(fileName = "SampleObjectsSO", menuName = "Collections/Sample Objects", order = 1)]
//public class SampleObjectsSO : ScriptableObject
//{
//    public List<SampleObject> Collection = new List<SampleObject>();
//}
