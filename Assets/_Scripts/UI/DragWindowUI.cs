using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindowUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private Vector2 _dragDirection, _xRange, _yRange;
    [SerializeField] private RectTransform _rectTransform;

    Vector2 _targetPos, _startPos, _offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = eventData.position - (Vector2)_rectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _targetPos = (eventData.position - _offset);
        var clampedPos = _targetPos;
        clampedPos.y = Mathf.Clamp(_targetPos.y, _startPos.y + _yRange.x, _startPos.y + _yRange.y);
        clampedPos.x = Mathf.Clamp(_targetPos.x, _startPos.x + _xRange.x, _startPos.x + _xRange.x);
        _rectTransform.localPosition = clampedPos * _dragDirection;
    }

    void Start()
    {
        _startPos = _rectTransform.localPosition;
    }
}
