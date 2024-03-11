using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public Vector2 FingerScreenPosition;
    public Vector3 FingerWorldPosition;
    
    [SerializeField] private List<string> _raycastLayers;

    private Vector3 _lastMousePosition;
    private LayerMask _layerMask;
    private Ray _ray;
    private RaycastHit _hit;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        foreach (string s in _raycastLayers)
        {
            _layerMask += LayerMask.GetMask(s);
        }
        LeanTouch.OnFingerDown += FingerDownHandler;
    }

    private void FingerDownHandler(LeanFinger finger)
    {
        //Debug.Log(finger.ScreenPosition);
        FingerScreenPosition = finger.ScreenPosition;
        Raycast(finger.ScreenPosition);
    }

    void Raycast(Vector2 v2)
    {
        _ray = _mainCamera.ScreenPointToRay(v2);

        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, _layerMask))
        {
            //Events.OnTouchRaycastMove?.Invoke(FingerWorldPosition);
            FingerWorldPosition = _hit.point;

            int layer = _hit.transform.gameObject.layer;

            switch (LayerMask.LayerToName(layer))
            {
                case "Grid":
                    if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

                    //Events.OnGridClicked?.Invoke();


                    break;

                case "Sequencer":
                    if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

                    if (_hit.transform.parent.TryGetComponent<Sequencer>(out var seq))
                    {

                        var step = _hit.transform.GetSiblingIndex() + 1;
                        Events.OnSequencerTapped?.Invoke(seq, step);
                    }

                    break;
            }

        }

        //Debug.DrawLine(mouseRay.origin, mouseRay.direction * 100, Color.red);
    }
}
