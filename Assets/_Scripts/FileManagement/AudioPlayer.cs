using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using FileManagement;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip Clip;

    public string LoadedPath;

    private void OnEnable()
    {
        Events.LoadPlayPath += LoadAndPlay;
    }

    private void OnDisable()
    {
        Events.LoadPlayPath += LoadAndPlay;
    }

    private async Task LoadAudio(string path)
    {
        Debug.Log("RELOADING AUDIO");
        bool isSamePath = path == LoadedPath;
        if (isSamePath) return;
        Debug.Log("loading new audio");

        var searchPath = path;

#if !UNITY_EDITOR
        searchPath = "File:///" + path;
#endif
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(searchPath, AudioType.WAV))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                Clip = DownloadHandlerAudioClip.GetContent(www);
                Source.clip = Clip;
                LoadedPath = path;
            }

            www.Dispose();
        }

    }

    public void PlayAudio()
    {
        Source.Play();
    }

    public async void LoadAudio()
    {
        await LoadAudio(FileManager.Instance.SelectedSamplePath);
    }

    public async void LoadAndPlay()
    {
        await LoadAudio(FileManager.Instance.SelectedSamplePath);
        Source.Play();
    }

    public async void LoadAndPlay(string path)
    {
        await LoadAudio(path);
        Source.Play();
    }

}
