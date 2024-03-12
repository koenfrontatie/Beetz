using Lean.Touch;
using TMPro;
using UnityEngine;

public class LeanTouchRaycasting : MonoBehaviour
{
    public Vector2 RaycastScreenPosition;
    
    private Camera _cam;
    private LayerMask _layerMask;
    private bool _isDragging;
    private LeanFinger _finger;

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += FingerTapHandler;
        LeanTouch.OnFingerDown += FingerDownHandler;
        LeanTouch.OnFingerOld += FingerHeldHandler;
        LeanTouch.OnFingerUp += FingerUpHandler;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= FingerTapHandler;
        LeanTouch.OnFingerDown -= FingerDownHandler;
        LeanTouch.OnFingerOld -= FingerHeldHandler;
        LeanTouch.OnFingerUp -= FingerUpHandler;
    }

    private void Start()
    {
        _cam = Camera.main;
        _layerMask += LayerMask.GetMask("Sequencer");
        _layerMask += LayerMask.GetMask("Grid");
    }

    private void Update()
    {
        if (!_isDragging || _finger.IsOverGui) return;

        FingerDragHandler();
    }

    void FingerDownHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            RaycastScreenPosition = v2;

            Events.OnNewRaycastScreenPosition?.Invoke(v2);

            //------------------------------------------------------------------------------------------- Grid Finger Down
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grid"))
            {
                _isDragging = true;
                Events.OnGridFingerDown?.Invoke();
            }
        }
    }

    void FingerUpHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            RaycastScreenPosition = v2;

            Events.OnNewRaycastScreenPosition?.Invoke(v2);

            //------------------------------------------------------------------------------------------- Grid Finger Up
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grid"))
            {
                _isDragging = false;
                Events.OnGridFingerUp?.Invoke();
            }
        }
    }

    void FingerTapHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            RaycastScreenPosition = v2;

            Events.OnNewRaycastScreenPosition?.Invoke(v2);

            //------------------------------------------------------------------------------------------- Sequencer Tapped
            if (hit.transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
            {

                var step = hit.transform.GetSiblingIndex() + 1;
                Events.OnSequencerTapped?.Invoke(seq, step);
            }
            //------------------------------------------------------------------------------------------- Grid Tapped
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Grid"))
            {

                Events.OnGridTapped?.Invoke();
            }
        }
    }

    void FingerDragHandler()
    {
        var v2 = _finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            RaycastScreenPosition = v2;

            Events.OnNewRaycastScreenPosition?.Invoke(v2);
            Events.OnDrag?.Invoke();

        }
    }

    void FingerHeldHandler(LeanFinger finger)
    {
        _finger = finger;

        if (finger.IsOverGui) return;

        var v2 = finger.ScreenPosition;

        if (Physics.Raycast(_cam.ScreenPointToRay(v2), out var hit, Mathf.Infinity, _layerMask))
        {
            RaycastScreenPosition = v2;

            Events.OnNewRaycastScreenPosition?.Invoke(v2);

            //------------------------------------------------------------------------------------------- Sequencer Held
            if (hit.transform.parent.parent.TryGetComponent<Sequencer>(out var seq))
            {

                var step = hit.transform.GetSiblingIndex() + 1;
                Events.OnSequencerHeld?.Invoke(seq);
            }
        }

    }
}
