using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SampleDataCollection", menuName = "Collections/SampleData", order = 5)]
public class SampleDataCollection : ScriptableObject
{
    public List<SampleData> Collection = new List<SampleData>();
}
