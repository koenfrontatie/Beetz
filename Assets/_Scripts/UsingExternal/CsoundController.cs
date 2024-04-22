using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CsoundController : MonoBehaviour
{
    public static CsoundController Instance;

    public CsoundUnity CsoundUnity;

    public List<string> queuedEvents = new List<string>();

    public List<string> channelNames = new List<string>();
    public List<Slider> sliders = new List<Slider>();
    public List<float> multipliers = new List<float>();



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
        SetControlChannels();

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

    void SetControlChannels()
    {
        if (!CsoundUnity.IsInitialized) return;
        if (channelNames.Count == 0 || sliders.Count == 0) return;
        
        for(int i = 0; i < channelNames.Count; i++)
        {
            CsoundUnity.SetChannel(channelNames[i], (double)sliders[i].value * multipliers[0]);
        }
    }

    //private void OnApplicationQuit()
    //{
    //    CsoundUnity.CsoundReset();
    //}

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
