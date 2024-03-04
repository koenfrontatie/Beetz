//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;

//public class InputControllerOld : MonoBehaviour
//{
//    public bool IsRaycasting;
//    public GameObject TargetedObject;
//    public Vector3 MouseWorldPosition;
//    [SerializeField] private Camera _mainCamera;
//    [SerializeField] private List<string> _raycastLayers;

   
//    private Vector3 _lastMousePosition;
//    private LayerMask _layerMask;
//    private Ray _mouseRay;
//    private RaycastHit _mouseHit;




//    private void OnEnable()
//    {
//        Events.OnGridStateChanged += (GameState state) => { RaycastMouse(); IsRaycasting = state == GameState.Patching ? true : false; };
//    }
//    void Start()
//    {
//        //_mainCamera = Camera.main;
//        foreach(string s in _raycastLayers)
//        {
//            _layerMask += LayerMask.GetMask(s);
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.UpdateState(GameState.Viewing);

//        if (IsMouseOverUI()) return;

        
//        if (!IsRaycasting) return;

//        if (Input.GetKeyDown(KeyCode.Mouse0))
//        {
//            RaycastMouse();
//        }

//        if (!MouseMoving()) return;

//        RaycastMouse();
//        Events.OnTouchRaycastMove?.Invoke(MouseWorldPosition);
//    }

//    bool MouseMoving()
//    {
//        if (_lastMousePosition != Input.mousePosition)
//        {
//            _lastMousePosition = Input.mousePosition;
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    void RaycastMouse()
//    {
//        _mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);

//        if (Physics.Raycast(_mouseRay, out _mouseHit, Mathf.Infinity, _layerMask))
//        {
//            MouseWorldPosition = _mouseHit.point;
           

//            int layer = _mouseHit.transform.gameObject.layer;

//            switch (LayerMask.LayerToName(layer))
//            {
//                case "Grid":
//                    if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            
//                    Events.OnGridClicked?.Invoke();


//                    break;
                
//                case "Sequencer":
//                    if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
                    
//                    if (_mouseHit.transform.parent.TryGetComponent<Sequencer>(out var seq))
//                    {

//                        var step = _mouseHit.transform.GetSiblingIndex() + 1;
//                        Events.OnSequencerClicked?.Invoke(seq, step);
//                    }

//                    break;
//            }
            
//        }

//        //Debug.DrawLine(mouseRay.origin, mouseRay.direction * 100, Color.red);
//    }

//    //void MouseInteraction()
//    //{
//    //    if (IsMouseOverUI()) return;

//    //    mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

//    //    if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetKeyDown(KeyCode.Mouse0))
//    //    {
//    //        if (Physics.Raycast(mouseRay, out mouseRayHit, 300f))
//    //        {
//    //            MouseWorldPosition = mouseRayHit.point;
//    //            Events.OnMouseWorldPosition?.Invoke(MouseWorldPosition);
//    //            //Debug.DrawRay(mainCamera.transform.position, MouseWorldPosition, Color.red);
//    //            Debug.DrawLine(mainCamera.transform.position, MouseWorldPosition, Color.red);
//    //            if(Input.GetKeyDown(KeyCode.Mouse0)) Events.OnLocationClicked?.Invoke(MouseWorldPosition);
//    //        }
//    //    }
//    //}

//    //if (MouseOnBorder())
//    //{
//    //    RaycastHit hit;
//    //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
//    //    if (Physics.Raycast(ray, out hit))
//    //    {
//    //        EventManager.OnNewCamPoint(hit.point);
//    //    }
//    //}

//    bool MouseOnBorder()
//    {
//        if (Input.mousePosition.y >= Screen.height * 0.95 || Input.mousePosition.y <= Screen.height * 0.05 || Input.mousePosition.x >= Screen.width * 0.95 || Input.mousePosition.x <= Screen.width * 0.05)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    private bool IsMouseOverUI()
//    {
//        return EventSystem.current.IsPointerOverGameObject();
//    }
//    private void OnDisable()
//    {
//        Events.OnGridStateChanged -= (GameState state) => { IsRaycasting = state == GameState.Patching ? true : false; };
//    }
//}


