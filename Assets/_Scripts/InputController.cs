using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public GameObject TargetedObject;

    void LateUpdate()
    {
        MouseInteraction();
    }

    void MouseInteraction()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && !IsMouseOverUI())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != null)
                {
                    TargetedObject = hit.transform.gameObject;
                    Events.OnLocationClicked?.Invoke(hit.point);
                    //TargetedObject = hit.transform.parent.transform.parent.gameObject; // the base object is the parent's parent < geometry < targetobject
                }
            }
        }

        //if (MouseOnBorder())
        //{
        //    RaycastHit hit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        EventManager.OnNewCamPoint(hit.point);
        //    }
        //}

    }

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
}

