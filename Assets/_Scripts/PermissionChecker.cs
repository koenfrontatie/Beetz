using UnityEngine.Android;
using UnityEngine;
using System.IO;

public class PermissionChecker : MonoBehaviour
{
    void Start()
    {
        // Check if the READ_EXTERNAL_STORAGE permission is granted
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            // Request READ_EXTERNAL_STORAGE permission
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        // Check if the WRITE_EXTERNAL_STORAGE permission is granted
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            // Request WRITE_EXTERNAL_STORAGE permission
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }


    //public static readonly string PersistentDataPath = Application.persistentDataPath;
    //public static readonly string StreamingBaseSamples = Path.Combine(Application.streamingAssetsPath, "BaseSamples");
    //public static readonly string SaveFilesPath = Path.Combine(PersistentDataPath, "SaveFiles");

    //public static readonly string ProjectSavepath = Path.Combine(SaveFilesPath, "Projects");
    //public static readonly string SampleSavepath = Path.Combine(SaveFilesPath, "Samples");
    //public static readonly string PersistentBaseSamples = Path.Combine(SampleSavepath, "BaseSamples");
    //public static readonly string SequencerSavepath = Path.Combine(SaveFilesPath, "Sequencers");
}
