using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SampleObjectCollection", menuName = "Collections/SampleObject", order = 1)]
public class SampleObjectCollection : ScriptableObject
{
    public List<SampleObject> Collection = new List<SampleObject>();
}
