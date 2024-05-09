using JetBrains.Annotations;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public DragDropUI InventoryDragDropUI;

    public string GUID;

    public void Bind(DragDropUI item)
    {
        item.transform.SetParent(transform.parent);
        item.transform.localPosition = Vector3.zero;

        InventoryDragDropUI = item;

        if(item.TryGetComponent<SampleObject>(out var so))
        {
            GUID = so.SampleData.ID;
        }

        item.transform.localScale = Vector3.one;
    }

    public void Clear()
    {
        if(InventoryDragDropUI != null)
        {
            Destroy(InventoryDragDropUI.gameObject);
            InventoryDragDropUI = null;
        }
    }
}
