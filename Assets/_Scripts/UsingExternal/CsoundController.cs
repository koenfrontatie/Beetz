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

    public void PlayTemplate(int i)
    {
        CsoundUnity.SendScoreEvent($"i {i} 0 6");
    }

    private void OnEnable()
    {
        Events.OnScoreEvent += CsoundUnity.SendScoreEvent;
    }

    private void OnDisable()
    {
        Events.OnScoreEvent -= CsoundUnity.SendScoreEvent;
    }

}
