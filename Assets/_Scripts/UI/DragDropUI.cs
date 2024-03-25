using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _startPosition;
    [SerializeField] private bool _resetPositionOnRelease = true;
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
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

        foreach (var hit in hits)
        {
            var droppedItem = hit.gameObject.GetComponent<DragDropUI>();

            if (droppedItem)
            {
                droppedItem.transform.position = _startPosition;
            }
        }

        foreach (var hit in hits)
        {
            var droppedContainer = hit.gameObject.GetComponent<InventorySlot>();

            if (droppedContainer)
            {
                _startPosition = droppedContainer.transform.position;
                transform.position = droppedContainer.transform.position;

                Events.OnInventoryChange?.Invoke();
            }
        }

        if (_resetPositionOnRelease)
            transform.position = _startPosition;
    }
}
