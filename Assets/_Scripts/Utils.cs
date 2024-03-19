using UnityEngine;
using System.Threading.Tasks;
using System.IO;

public static class Utils
{
    private static readonly string PersistentDataPath = Application.persistentDataPath;
    private static readonly string StreamingAssetsPath = Application.streamingAssetsPath;
    public static readonly string BaseSamplesPath = $"{StreamingAssetsPath}";
    private static readonly string SaveFilesPath = $"{PersistentDataPath}{Path.DirectorySeparatorChar}SaveFiles";
    
    public static readonly string ProjectSavepath = $"{SaveFilesPath}{Path.DirectorySeparatorChar}Projects";
    public static readonly string SampleSavepath = $"{SaveFilesPath}{Path.DirectorySeparatorChar}Samples";
    public static readonly string SequencerSavepath = $"{SaveFilesPath}{Path.DirectorySeparatorChar}Sequencers";
    //public static readonly string SaveFilesSolarSystems = SaveFilesPath + "/SolarSystems";
    //public static readonly string SaveFilesSolarSystemsScreenshots = SaveFilesSolarSystems + "/screenshots







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
