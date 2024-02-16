using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SampleManager : MonoBehaviour
{
    public static SampleManager Instance;
    //private string BaseSampleDirectory;    
    public List<AudioClip> BaseSamples = new List<AudioClip>();

    public SampleObject SelectedSample;

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

        SearchDirectories();
    }

    private void Start()
    {
        Events.OnHotbarClicked += (i) => SelectedSample = Prefabs.Instance.BaseObjects[i];
    }

    private void SearchDirectories()
    {
        if (Directory.Exists(Utils.BaseSamplesPath))
        {
            DirectoryInfo info = new DirectoryInfo(Utils.BaseSamplesPath);

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
