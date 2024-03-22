using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Button _button;

    private string _guid;

    bool DroppedInContainer(PointerEventData eventData)
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        foreach(var hit in hits)
        {
            var droppedContainer = hit.gameObject.GetComponent<InventorySlot>();

            if(droppedContainer)
            {
                Debug.Log($"Dragended on {droppedContainer.name}");

                //swap

                return true;
            }
        }

        return false;
       
    }
}
