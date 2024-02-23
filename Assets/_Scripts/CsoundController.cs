using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsoundController : MonoBehaviour
{
    public static CsoundController Instance;

    public CsoundUnity CsoundUnity;

    private void Awake()
    {
        Instance = this;
        CsoundUnity = GetComponent<CsoundUnity>();
    }



}
