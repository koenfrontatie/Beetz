using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _startPosition;
    [SerializeField] private bool _resetPositionOnRelease = true;
    //public void OnDrag(PointerEventData eventData)
    //{
    //    transform.position = eventData.position;
    //}
    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 vec = Camera.main.WorldToScreenPoint(transform.position);
        //vec.x += eventData.delta.x;
        //vec.y += eventData.delta.y;
        transform.position = eventData.position;
        //myRectTransform.anchoredPosition = eventData.position / myCanvas.scaleFactor;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_resetPositionOnRelease)
            _startPosition = transform.position;

        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, hits);

        DragDropUI swapWith = null;

        foreach (var hit in hits)
        {
            var droppedItem = hit.gameObject.GetComponent<DragDropUI>();

            if (droppedItem && droppedItem != this)
            {
                droppedItem.transform.position = _startPosition;

                swapWith = droppedItem;
                break; 

            }
        }

        foreach (var hit in hits)
        {
            var droppedContainer = hit.gameObject.GetComponent<InventorySlot>();

            if (droppedContainer)
            {
                _startPosition = droppedContainer.transform.position;
                transform.position = droppedContainer.transform.position;

               
                if(swapWith != null)
                {
                    Events.ItemSwap?.Invoke(droppedContainer, this, swapWith);
                    Events.OnInventoryChange?.Invoke();

                }
                
            }
        }

        if (_resetPositionOnRelease)
            transform.position = _startPosition;
    }
}
