using UnityEngine;
using System.Threading.Tasks;
using System.IO;

public static class Utils
{
    public static readonly string PersistentDataPath = Application.persistentDataPath;
    public static readonly string StreamingBaseSamples = Path.Combine(Application.streamingAssetsPath, "basesamples");
    public static readonly string SaveFilesPath = Path.Combine(PersistentDataPath, "SaveFiles");

    public static readonly string ProjectSavepath = Path.Combine(SaveFilesPath, "Projects");
    public static readonly string SampleSavepath = Path.Combine(SaveFilesPath, "UniqueSamples");
    public static readonly string PersistentBaseSamples = Path.Combine(SaveFilesPath, "BaseSamples");
    //public static readonly string SequencerSavepath = Path.Combine(SaveFilesPath, "Sequencers");
    //public static readonly string SaveFilesSolarSystems = SaveFilesPath + "/SolarSystems";
    //public static readonly string SaveFilesSolarSystemsScreenshots = SaveFilesSolarSystems + "/screenshots



    public static bool CheckForCreateDirectory(string directory)
    {
        if (Directory.Exists(directory)) return true;
        Debug.Log("Creating dir " + directory);
        Directory.CreateDirectory(directory);
        return false;
    }


    public static bool CheckForFile(string path)
    {
        if (File.Exists(path)) return true;
        return false;
    }

    public async static Task LerpToTarget(GameObject obj, Vector3 targetPosition, float duration)
    {
        var LerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        Vector3 initialPosition = obj.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = LerpCurve.Evaluate(t);
            obj.transform.position = Vector3.Lerp(initialPosition, targetPosition, curveValue);
            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }

        obj.transform.position = targetPosition;
    }
}
