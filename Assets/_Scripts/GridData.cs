using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridData : MonoBehaviour
{
    const int COLUMS = 160;
    const int ROWS = 80;

    public string[,] GridArray = new string[COLUMS, ROWS];

    private void Start()
    {
        //GridArray[0, 5] = "Sup";
        //GridArray[1, 5] = "sex";

        ////Debug.Log(GridArray);
        ////Debug.Log(GridArray[0, 5]);
        ////Debug.Log(GridArray[1, 5]);

        //GridArray[COLUMS - 1, ROWS - 1] = "sex1";

        //LogArray(GridArray);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;

    //    for (int i = 0; i < GridArray.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < GridArray.GetLength(1); j++)
    //        {
    //            Gizmos.DrawSphere(new Vector3(i, 0, j), .1f);
    //        }
    //    }
    //}

    public void LogArray(string[,] array)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                sb.Append(array[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
}
