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


    public async void AssignObject()
    {
        var replace = _librarySlot.InventoryDragDropUI;

        if (replace != null)
        {

#if UNITY_EDITOR
            DestroyImmediate(_librarySlot.InventoryDragDropUI.gameObject);
#else
            Destroy(_librarySlot.InventoryDragDropUI.gameObject);
#endif
            _librarySlot.InventoryDragDropUI = null;
        }

        var item = await AssetBuilder.Instance.GetToolbarItem(_guid);

        _librarySlot.Bind(item.GetComponent<DragDropUI>());

        SetNameText(item.GetComponent<SampleObject>().SampleData.Name);

        item.transform.localScale = Vector3.one;
    }

    public async void AssignSampleData(string guid)
    {
        //var sampleObject = AssetBuilder.Instance.CustomSamples.GetSampleObject(guid);
        //_inventorySlot.Bind(sampleObject);
        //SetNameText(sampleObject.Name);


        var item = await AssetBuilder.Instance.GetToolbarItem(guid);

        var sampleData = await AssetBuilder.Instance.GetSampleData(guid);

        _guid = sampleData.ID;

        item.GetComponent<SampleObject>().SampleData = sampleData;

        _librarySlot.Bind(item.GetComponent<DragDropUI>());

        SetNameText(sampleData.Name);

        item.transform.localScale = Vector3.one;
    }
//    public void Bind(SampleObject object)
//    {
//=        SetNameText(slot.SampleObject.Name);
//    }
}
