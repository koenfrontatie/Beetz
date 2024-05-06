using System;
using UnityEngine;
using Un4seen.Bass;

public class BassManager : MonoBehaviour
{
    bool isInitialized = false;
    string statusUpdateString = "";

    void Start()
    {
        //Debug.Log(Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero));
        try
        {
            //BassNet.Registration("zippo227@gmail.com", "test");

            isInitialized = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!isInitialized)
            {
                statusUpdateString = "Bass_Init error!";
            }
            else
            {
                statusUpdateString = "Bass_Init success!";
            }

            // init your recording device (we use the default device)
            //if (!Bass.BASS_RecordInit(-1))
            //{
            //	statusUpdateString = "Bass_RecordInit error!";
            //}
        }
        catch (Exception ex)
        {
            statusUpdateString = $"Bass_Init error! {ex.Message}";
        }
    }

    void OnDestroy()
    {
        Bass.BASS_Free();
    }
}
