using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EyePlacer : MonoBehaviour
{
    public Transform _object;

    public Transform _eye1, _eye2;

    public RaycastHit hit1, hit2;

    public void PositionEyes()
    {
        _eye1.LookAt(_object);
        _eye2.LookAt(_object);

        if(Physics.Raycast(_eye1.position, _eye1.forward * 2, out RaycastHit hit1))
        {
            //Debug.Log("hit1: " + hit1.point);
            _eye1.position = hit1.point;
        };
        if (Physics.Raycast(_eye2.position, _eye2.forward * 2, out RaycastHit hit2))
        {
            //Debug.Log("hit1: " + hit1.point);
            _eye2.position = hit2.point;
        };

        //Debug.DrawRay(_eye1.position, _eye1.forward, Color.green);

        //Debug.DrawRay(_eye2.position, _eye2.forward, Color.green);


        //DrawGizmo(hit1.position, hit1.point, Color.red);
        //DrawGizmo(hit1.position, hit1.point, Color.red);

    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_eye1.position, _eye1.forward, Color.green);

        Debug.DrawRay(_eye2.position, _eye2.forward, Color.green);
        Gizmos.color = Color.black;
        //Gizmos.DrawSphere(hit1.point, .1f);
        //Gizmos.DrawSphere(hit2.point, .1f);
    }
}
