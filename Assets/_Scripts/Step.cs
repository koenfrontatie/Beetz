using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void SetMat(Material mat)
    {
        meshRenderer.material = mat;
    }
}
