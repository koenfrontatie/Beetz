using JetBrains.Annotations;
using UnityEngine;

public class LibrarySlot : MonoBehaviour
{
    public DragDropUI InventoryDragDropUI;

    public string GUID;

    public void Bind(DragDropUI item)
    {
        item.transform.SetParent(transform.parent);
        item.transform.position = transform.position;

        InventoryDragDropUI = item;

        if (item.TryGetComponent<SampleObject>(out var so))
        {
            GUID = so.SampleData.ID;
        }
    }
}
