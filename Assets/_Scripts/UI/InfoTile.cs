using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTile : MonoBehaviour
{
    [SerializeField] LibrarySlot _librarySlot;
    [SerializeField] TMPro.TextMeshProUGUI _nameText;

    void SetNameText(string text)
    {
        _nameText.text = text;
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
