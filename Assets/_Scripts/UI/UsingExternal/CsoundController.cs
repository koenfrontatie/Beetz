//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CsoundController : MonoBehaviour
//{
//    public static CsoundController Instance;

//    public CsoundUnity CsoundUnity;

//    public List<string> queuedScoreEvents = new List<string>();

//    public List<string> channelNames = new List<string>();
//    public List<Slider> sliders = new List<Slider>();
//    public List<float> multipliers = new List<float>();



//    private void Awake()
//    {
//        Instance = this;
//        CsoundUnity = GetComponent<CsoundUnity>();
//    }

//    public void SendGuidToQueue(string guid)
//    {
//        for (int i = 0; i < queuedScoreEvents.Count; i++)
//        {
//            if (guid == queuedScoreEvents[i]) return;
//        }

//        string eventString; // ---------- turns guid into score event

//        if (guid.Length < 3) // if template
//        {
//            eventString = $"i {int.Parse(guid) + 1} 0 6";
//        }
//        else
//        {
//            eventString = $"i {guid} 0 6";
//        }

//        for (int i = 0; i < queuedScoreEvents.Count; i++)
//        {
//            if (eventString == queuedScoreEvents[i]) return;
//        }

//        queuedScoreEvents.Add(eventString);
//    }

//    //void SendScoreEvent()
//    //{
//        //Events.OnQueueScoreEvent?.Invoke($"i {(int)(_sampleObject.SampleData.Template + 1)} 0 6");
//        //CsoundController.Instance.SendEventToQueue

//    //}

//    private void LateUpdate()
//    {
//        SetControlChannels();

//        if (queuedScoreEvents.Count > 0)
//        {
//            PlayQueue();
//        }
//    }

//    public void PlayQueue()
//    {
//        for (int i = 0; i < queuedScoreEvents.Count; i++)
//        {
//            CsoundUnity.SendScoreEvent(queuedScoreEvents[i]);
//        }

//        queuedScoreEvents.Clear();
//    }

//    void SetControlChannels()
//    {
//        if (!CsoundUnity.IsInitialized) return;
//        if (channelNames.Count == 0 || sliders.Count == 0) return;
        
//        for(int i = 0; i < channelNames.Count; i++)
//        {
//            CsoundUnity.SetChannel(channelNames[i], (float)sliders[i].value * multipliers[0]);
//        }
//    }

//    //private void OnApplicationQuit()
//    //{
//    //    CsoundUnity.CsoundReset();
//    //}

//    private void OnEnable()
//    {
//        Events.QueueForPlayback += SendGuidToQueue;
//    }

//    private void OnDisable()
//    {
//        Events.QueueForPlayback -= SendGuidToQueue;
//    }

//}
