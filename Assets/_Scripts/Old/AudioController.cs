//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using NAudio;
//using NAudio.Wave;


//[RequireComponent(typeof(AudioSource))]
//public class AudioController : MonoBehaviour
//{
//    private AudioSource source;

//    private WaveOutEvent waveOut;

//    private void OnEnable()
//    {
//        Events.OnHotbarClicked += TriggerSample;
//        Events.OnSampleTrigger += TriggerSample;
//        //Events.OnSampleTriggerUrl += PlayClipNAudio;
//    }
//    void Start()
//    {
//        source = GetComponent<AudioSource>();
//    }

//    private void Update()
//    {
//        if (waveOut != null && waveOut.PlaybackState == PlaybackState.Stopped)
//        {
//            // Clean up resources
//            waveOut.Dispose();
//            waveOut = null;
//        }
//    }
//    void PlayClip(AudioClip clip)
//    {
//        source.PlayOneShot(clip);
//    }

//    //void PlayClipNAudio(string path)
//    //{
//    //    WaveFileReader waveFileReader = new WaveFileReader(path);
//    //    waveOut = new WaveOutEvent();
//    //    waveOut.Init(waveFileReader);
//    //    waveOut.Volume = .5f;
//    //    waveOut.Play();
//    //}

//    void TriggerSample(string id)
//    {
//        if (id.Length < 3)
//        {
//            var choice = int.Parse(id);
//            PlayClip(SampleManager.Instance.BaseSamples[choice]);
//            //Debug.Log(SampleManager.Instance.BaseSamples[choice]);
//        }
//    }

//    void TriggerSample(int id)
//    {
//        if (id < 100)
//        {
//            PlayClip(SampleManager.Instance.BaseSamples[id]);
//        }
//    }
//}
 