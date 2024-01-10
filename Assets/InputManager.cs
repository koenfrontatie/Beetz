using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public GameObject TargetedObject;
    public UnityAction<Vector3> OnLocationClicked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void LateUpdate()
    {
        MouseInteraction();
    }

    void MouseInteraction()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject != null)
                {
                    TargetedObject = hit.transform.gameObject;
                    OnLocationClicked?.Invoke(hit.point);
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
}

