using UnityEngine;

public class LineMesh : MonoBehaviour
{
    [SerializeField] private float _length;
    [SerializeField] private float _width;
    [SerializeField] private MeshRenderer _meshRenderer;

    public void UpdateLineMesh()
    {
        //Debug.Log("UpdateLineMesh");
        transform.localScale = new Vector3(_width, 1, _length);
    }

    public void SetLength(float newLength)
    {
        _length = newLength;
        UpdateLineMesh();
    }

    public void SetWidth(float newWidth)
    {
        _width = newWidth;
        UpdateLineMesh();
    }

    public void SetColor(Color color)
    {
        _meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material.color = color;
        _meshRenderer.material.SetColor("_EmissionColor", color);
    }
}
