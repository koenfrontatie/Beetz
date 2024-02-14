using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public bool IsRaycasting;
    public GameObject TargetedObject;
    public Vector3 MouseWorldPosition;

    private Camera mainCamera;
    private Vector3 lastMousePosition;

    [SerializeField] List<string> raycastLayers;
    private LayerMask layerMask;

    private Ray mouseRay;
    private RaycastHit mouseHit;

    private void OnEnable()
    {
        Events.OnGameStateChanged += (GameState state) => { RaycastMouse(); IsRaycasting = state == GameState.Placing ? true : false; };
    }
    void Start()
    {
        mainCamera = Camera.main;
        foreach(string s in raycastLayers)
        {
            layerMask += LayerMask.GetMask(s);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.UpdateState(GameState.Viewing);

        if (IsMouseOverUI()) return;

        
        if (!IsRaycasting) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastMouse();
            Events.OnLocationClicked?.Invoke(MouseWorldPosition);
        }

        if (!MouseMoving()) return;

        RaycastMouse();
    }

    bool MouseMoving()
    {
        if (lastMousePosition != Input.mousePosition)
        {
            lastMousePosition = Input.mousePosition;
            return true;
        }
        else
        {
            return false;
        }
    }

    void RaycastMouse()
    {
        mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, layerMask))
        {
            MouseWorldPosition = mouseHit.point;

            var layer = mouseHit.transform.gameObject.layer;

            switch (LayerMask.LayerToName(layer))
            {
                case "Grid":
                    Events.OnMouseRaycastGrid?.Invoke(MouseWorldPosition);
                    break;
                case "Sequencer":
                    Events.OnMouseRaycastGrid?.Invoke(MouseWorldPosition);
                    break;
            }
        }

        //Debug.DrawLine(mouseRay.origin, mouseRay.direction * 100, Color.red);
    }

    //void MouseInteraction()
    //{
    //    if (IsMouseOverUI()) return;

    //    mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);

    //    if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        if (Physics.Raycast(mouseRay, out mouseRayHit, 300f))
    //        {
    //            MouseWorldPosition = mouseRayHit.point;
    //            Events.OnMouseWorldPosition?.Invoke(MouseWorldPosition);
    //            //Debug.DrawRay(mainCamera.transform.position, MouseWorldPosition, Color.red);
    //            Debug.DrawLine(mainCamera.transform.position, MouseWorldPosition, Color.red);
    //            if(Input.GetKeyDown(KeyCode.Mouse0)) Events.OnLocationClicked?.Invoke(MouseWorldPosition);
    //        }
    //    }
    //}

    //if (MouseOnBorder())
    //{
    //    RaycastHit hit;
    //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        EventManager.OnNewCamPoint(hit.point);
    //    }
    //}

    bool MouseOnBorder()
    {
        if (Input.mousePosition.y >= Screen.height * 0.95 || Input.mousePosition.y <= Screen.height * 0.05 || Input.mousePosition.x >= Screen.width * 0.95 || Input.mousePosition.x <= Screen.width * 0.05)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    private void OnDisable()
    {
        Events.OnGameStateChanged -= (GameState state) => { IsRaycasting = state == GameState.Placing ? true : false; };
    }
}


