using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CsoundController : MonoBehaviour
{
    public static CsoundController Instance;

    public CsoundUnity CsoundUnity;

    public List<string> queuedEvents = new List<string>();


    private void Awake()
    {
        Instance = this;
        CsoundUnity = GetComponent<CsoundUnity>();
    }

    public void PlayTemplate(int i)
    {
        CsoundUnity.SendScoreEvent($"i {i} 0 6");
    }

    public void SendEventToQueue(string eventString)
    {
        for (int i = 0; i < queuedEvents.Count; i++)
        {
            if (eventString == queuedEvents[i]) return;
        }

        queuedEvents.Add(eventString);

    }

    private void LateUpdate()
    {
        if (queuedEvents.Count > 0)
        {
            PlayQueue();
        }
    }

    public void PlayQueue()
    {
        for (int i = 0; i < queuedEvents.Count; i++)
        {
            CsoundUnity.SendScoreEvent(queuedEvents[i]);
        }

        queuedEvents.Clear();
    }

    private void OnEnable()
    {
        Events.OnScoreEvent += CsoundUnity.SendScoreEvent;
        Events.OnQueueScoreEvent += SendEventToQueue;
    }

    private void OnDisable()
    {
        Events.OnScoreEvent -= CsoundUnity.SendScoreEvent;
        Events.OnQueueScoreEvent -= SendEventToQueue;
    }

}
