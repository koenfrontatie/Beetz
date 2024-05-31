using UnityEngine.Networking;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using FileManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource _source;
    public Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>(); 
    void OnEnable()
    {
        Events.LoadPlayGuid += PlayFromGuid;
        FileManager.SampleUpdated += OnSampleUpdated;
    }
    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    private async void PlayFromGuid(string guid)
    {
        var clip = await GetClip(guid);

        if (clip != null)
        {
            _source.PlayOneShot(clip);
        }
        else
        {
            string template = await FileManager.Instance.TemplateFromGuid(guid);
            clip = await GetClip(template);
            _source.PlayOneShot(clip);
        }
    }

    //private async Task<AudioClip> GetClip(string guid)
    //{
    //    if (_clipDictionary.ContainsKey(guid)) {
    //        return _clipDictionary[guid];
    //    } else
    //    {
    //        var clip = await LoadClipFromPath(FileManager.Instance.SamplePathFromGuid(guid));
    //        _clipDictionary.Add(guid, clip);
    //        return clip;
    //    }
    //}

    private readonly object _clipDictionaryLock = new object();

    private async Task<AudioClip> GetClip(string guid)
    {
        // Check if the clip is already in the dictionary
        lock (_clipDictionaryLock)
        {
            if (_clipDictionary.ContainsKey(guid))
            {
                return _clipDictionary[guid];
            }
        }

        // Load the clip outside of the lock to avoid long blocking
        var clip = await LoadClipFromPath(FileManager.Instance.SamplePathFromGuid(guid));

        // Add the loaded clip to the dictionary
        lock (_clipDictionaryLock)
        {
            if (!_clipDictionary.ContainsKey(guid)) // Double-check inside the lock
            {
                _clipDictionary.Add(guid, clip);
            }
        }

        return clip;
    }

    private async Task<AudioClip> LoadClipFromPath(string path)
    {
        var searchPath = path;
        AudioClip Clip = null;
#if !UNITY_EDITOR
    searchPath = "File:///" + path;
#endif
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(searchPath, AudioType.WAV))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Clip = DownloadHandlerAudioClip.GetContent(www);
                
            }

            www.Dispose();

            return Clip;
        }
    }

    private async void OnSampleUpdated(string guid)
    {
        if(_clipDictionary.ContainsKey(guid))
        {
            _clipDictionary.Remove(guid);
        }

        await GetClip(guid);
    }

    void OnDisable()
    {
        Events.LoadPlayGuid -= PlayFromGuid;
        FileManager.SampleUpdated -= OnSampleUpdated;

    }
}


