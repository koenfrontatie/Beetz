using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SampleManager : MonoBehaviour
{
    public static SampleManager Instance;
    private string BaseSampleDirectory;    
    public List<AudioClip> BaseSamples = new List<AudioClip>();

    public AudioClip SelectedSample;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        BaseSampleDirectory = $"{Application.streamingAssetsPath}{Path.DirectorySeparatorChar}Samples";
        UpdatePaths();
    }

    private void Start()
    {
        Events.OnHotbarClicked += (i) => SelectedSample = BaseSamples[i];
    }

    void UpdatePaths()
    {
        if (Directory.Exists(BaseSampleDirectory))
        {
            DirectoryInfo info = new DirectoryInfo(BaseSampleDirectory);

            FileInfo[] files = info.GetFiles().OrderBy(p => p.Name).ToArray();
            foreach (FileInfo file in files)
            {
                if (file.Name.EndsWith(".wav"))
                {
                    StartCoroutine(LoadAudio(file.FullName, file.Name));
                }
            }
        }
    }

    private IEnumerator LoadAudio(string path, string name)
    {
        //Debug.Log("RELOADING AUDIO");
        //Debug.Log(path);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                clip.name = name;
                BaseSamples.Add(clip);
            }
        }
    }
}
