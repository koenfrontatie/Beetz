using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureCollection", menuName = "Collections/Texture", order = 5)]
public class TextureCollection : ScriptableObject
{
    public List<Texture2D> Collection = new List<Texture2D>();
}
