using UnityEngine.Networking;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using FileManagement;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public AudioMixer AudioMixer;

    public bool SendReverb;

    private AudioSource _masterSource, _reverbSource;

    [SerializeField]
    private List<AudioSource> _reverb = new List<AudioSource>();
    [SerializeField]
    private DSPController _dspController;

    public Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>(); 
    void OnEnable()
    {
        Events.LoadPlaySample += PlayFromSampleData;
        FileManager.SampleUpdated += OnSampleUpdated;
    }
    void Start()
    {
        _masterSource = GetComponent<AudioSource>();
        _reverbSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    private async void PlayFromSampleData(SampleData sampleData)
    {
        AudioClip clip;

        //if (GameManager.Instance.State != GameState.Biolab)
        //{
            clip = await GetClip(sampleData.ID);
        //} else
        //{
        //    clip = await GetClip(sampleData.Template.ToString());
        //}

        if (GameManager.Instance.State == GameState.Biolab)
        {
            sampleData.Effects = _dspController.ActiveEffects;
        }
            // check for reverb
        float effectValue = 0f;

        for (int i = 0; i < sampleData.Effects.Count; i++)
        {
            if (sampleData.Effects[i].Effect == EffectType.Chorus)
            {
                SendReverb = true;
                effectValue = sampleData.Effects[i].Value;
                break;
            }
        }

        if (clip != null)
        {

            if(SendReverb)
            {
                HandleReverbSend(clip, effectValue);
                SendReverb = false;
            }

            _masterSource.PlayOneShot(clip);
        }
        else
        {
            clip = await GetClip(sampleData.Template.ToString());
            
            if (SendReverb)
            {
                HandleReverbSend(clip, effectValue);
                SendReverb = false;
            }

            _masterSource.PlayOneShot(clip);
        }
    }
    
    void HandleReverbSend(AudioClip clip, float value)
    {
        if (value <= .2f)
        {
            _reverb[0].PlayOneShot(clip);
            return;

        }
        else if (value <= .4f)
        {
            _reverb[1].PlayOneShot(clip);
            return;

        }
        else if (value <= .6f)
        {
            _reverb[2].PlayOneShot(clip);
            return;

        }
        else if (value <= .8f)
        {
            _reverb[3].PlayOneShot(clip);
            return;
        }
        else if (value <= 1f)
        {
            _reverb[4].PlayOneShot(clip);
            return;
        }

        _masterSource.PlayOneShot(clip);
    }
    void HandleEffectSend(AudioClip clip, EffectValuePair ev)
    {
        switch(ev.Effect)
        {
            case EffectType.Reverb:
                _reverbSource.PlayOneShot(clip);

                if (ev.Value <= .2f)
                {
                    _reverb[0].PlayOneShot(clip);
                    return;

                }
                else if (ev.Value <= .4f)
                {
                    _reverb[1].PlayOneShot(clip);
                    return;

                }
                else if (ev.Value <= .6f)
                {
                    _reverb[2].PlayOneShot(clip);
                    return;

                }
                else if (ev.Value <= .8f)
                {
                    _reverb[3].PlayOneShot(clip);
                    return;
                }
                else if (ev.Value <= 1f)
                {
                    _reverb[4].PlayOneShot(clip);
                    return;
                }

                break;
        }

        _masterSource.PlayOneShot(clip);
    }

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
        AudioClip clip = await LoadClipFromPath(FileManager.Instance.SamplePathFromGuid(guid));

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
        AudioClip Clip = null;
        if (Application.platform != RuntimePlatform.WindowsPlayer)
            path = "File:///" + path;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
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
        Events.LoadPlaySample -= PlayFromSampleData;
        FileManager.SampleUpdated -= OnSampleUpdated;

    }
}


