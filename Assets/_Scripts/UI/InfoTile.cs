using UnityEngine;

public class InfoTile : MonoBehaviour
{
    [SerializeField] private LibrarySlot _librarySlot;
    [SerializeField] private string _guid = "";
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private bool _isLocked;

    void SetNameText(string text)
    {
        _nameText.text = text;
    }


    public void AssignObject()
    {
        //var sampleObject = AssetBuilder.Instance.CustomSamples.GetSampleObject(guid);
        //_inventorySlot.Bind(sampleObject);
        //SetNameText(sampleObject.Name);
        var replace = _librarySlot.InventoryDragDropUI;

        if (replace != null)
        {

#if UNITY_EDITOR
            DestroyImmediate(replace.transform.gameObject);
#else
            Destroy(replace.transform.gameObject);
#endif
            //_librarySlot.InventoryDragDropUI = null;
        }

        var item = AssetBuilder.Instance.GetToolbarItem(_guid);

        _librarySlot.Bind(item.GetComponent<DragDropUI>());

        SetNameText(item.GetComponent<SampleObject>().SampleData.Name);

        item.transform.localScale = Vector3.one;
    }

    public void AssignObject(string guid)
    {
        //var sampleObject = AssetBuilder.Instance.CustomSamples.GetSampleObject(guid);
        //_inventorySlot.Bind(sampleObject);
        //SetNameText(sampleObject.Name);

        var item = AssetBuilder.Instance.GetToolbarItem(guid);

        _librarySlot.Bind(item.GetComponent<DragDropUI>());

        SetNameText(item.GetComponent<SampleObject>().SampleData.Name);

        item.transform.localScale = Vector3.one;
    }
//    public void Bind(SampleObject object)
//    {
//=        SetNameText(slot.SampleObject.Name);
//    }
}
