using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Android;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace FileManagement
{
    public class FileManager : MonoBehaviour
    {
        //project variables
        public string ProjectGuid = "1";
        public string SelectedSamplePath;
        //paths
        public string BaseSamplesDirectory;
        public List<string> BaseSamplesPathCollection = new List<string>();
        public string UniqueSamplesDirectory;
        public List<string> UniqueSamplesPathCollection = new List<string>();
        //events
        public static Action BaseSamplesInitialized;
        public static Action UniqueSamplesInitialized;
        public static Action<string> NewSampleSelected;

        void OnEnable()
        {
            Events.ProjectDataLoaded += (data) =>
            {
                ProjectGuid = data.ID;
                InitializeSamples();
            };

            //Events.SetSelectedSample += (so) =>
            //{
            //    SetPathFromGuid(so.SampleData.ID);
            //};

            Events.SetSelectedGuid += (guid) =>
            {
                SetPathFromGuid(guid);
            };
        }

        void OnDisable()
        {
            Events.ProjectDataLoaded -= (data) =>
            {
                ProjectGuid = data.ID;
                InitializeSamples();
            };

            //Events.SetSelectedSample -= (so) =>
            //{
            //    SetPathFromGuid(so.SampleData.ID);
            //};

            Events.SetSelectedGuid -= (guid) =>
            {
                SetPathFromGuid(guid);
            };
        }

        #region SampleInitialization
        async void InitializeSamples()
        {
            CheckAndroidPermissions();

            BetterStreamingAssets.Initialize();

            // --------------------------------------------------- directory initialization

            //await CheckOrMakeDirectory(Path.Combine(Application.streamingAssetsPath, "BaseSamples"));

            BaseSamplesDirectory = Path.Combine(Application.persistentDataPath, "BaseSamples");
            
            //Debug.Log($"BaseSamplesDirectory {BaseSamplesDirectory}");
            await CheckOrMakeDirectory(BaseSamplesDirectory);

            UniqueSamplesDirectory = Path.Combine(Application.persistentDataPath, "SaveFiles", "UniqueSamples", ProjectGuid);

            await CheckOrMakeDirectory(UniqueSamplesDirectory);
            //Debug.Log($"UniqueSamplesDirectory {BaseSamplesDirectory}");
            await InitializeBaseSamples();

            await InitializeUniqueSamples();

        }

        async Task CheckOrMakeDirectory(string directory)
        {
            if (Directory.Exists(directory)) return;

            await Task.Run(() =>
            {
                //Debug.Log("Creating dir " + directory);
                Directory.CreateDirectory(directory);
            });
        }
        public async void RefreshBase()
        {
            await InitializeUniqueSamples();
        }
        async Task InitializeBaseSamples()
        {
            // --------------------------------------------------- make sure all base samples in streaming assets are in persistent data path

            var betterfiles = BetterStreamingAssets.GetFiles("BaseSamples", "*.wav", SearchOption.AllDirectories);
            BaseSamplesPathCollection.Clear();

            for (int i = 0; i < betterfiles.Length; i++)
            {
                var pathToCheck = Path.Combine(Application.persistentDataPath, betterfiles[i]);
                BaseSamplesPathCollection.Add(pathToCheck);
                //Debug.Log($"Checking {betterfiles[i]}");
                await Task.Run(() =>
                {
                    if (!File.Exists(pathToCheck))
                    {
                        Debug.Log($"Didn't find {betterfiles[i]}, copying");
                        var data = BetterStreamingAssets.ReadAllBytes(betterfiles[i]);
                        //Debug.Log($"writing {betterfiles[i]}");
                        File.WriteAllBytes(pathToCheck, data);
                    }
                });
            }

            BaseSamplesInitialized?.Invoke();
        }

        public async void RefreshUnique()
        {
            await InitializeUniqueSamples();

        }
        async Task InitializeUniqueSamples()
        {
            UniqueSamplesPathCollection.Clear();

            // --------------------------------------------------- get all unique sample folders

            await Task.Run(() =>
            {
                var info = new DirectoryInfo(UniqueSamplesDirectory);

                var samplefolders = info.GetDirectories();

                for (int i = 0; i < samplefolders.Length; i++)
                {
                    var files = samplefolders[i].GetFiles();

                    for (int j = 0; j < files.Length; j++)
                    {
                        if(files[j].Name.EndsWith(".wav"))
                            UniqueSamplesPathCollection.Add(files[j].FullName);
                    }
                }
            });

            UniqueSamplesInitialized?.Invoke();
        }

        async Task MakeSamplePathIntoUnique(string path)
        {
            //var filename = Path.GetFileName(path);
            
            var newName = SaveLoader.Instance.NewGuid();
            
            var newSampleDirectory = Path.Combine(UniqueSamplesDirectory, newName);
            
            var newFilePath = Path.Combine(newSampleDirectory, newName + ".wav");

            await CheckOrMakeDirectory(newSampleDirectory);

            await Task.Run(() =>
            {
                if (!File.Exists(Path.Combine(newFilePath)))
                {
                    //var data = BetterStreamingAssets.ReadAllBytes(path);
                    var data = File.ReadAllBytes(path);
                    // create new unique sample folder
                    //var newPath = Path.Combine(UniqueSamplesDirectory, Path.GetFileName(path));
                    // copy sample file into new folder
                    File.WriteAllBytes(Path.Combine(newFilePath), data);
                }
            });
        }

        public async void MakeSelectedIntoUnique()
        {
            await MakeSamplePathIntoUnique(SelectedSamplePath);
            RefreshUnique();
        }

        void CheckAndroidPermissions()
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

        #endregion

        public void SetSelectedSamplePath(string path)
        {
            SelectedSamplePath = path;
            NewSampleSelected?.Invoke(SelectedSamplePath);
        }

        public void SetPathFromGuid(string guid)
        {
            if(guid.Length < 3)
            {
                SelectedSamplePath = BaseSamplesPathCollection[int.Parse(guid)];
                Events.BaseSampleSelected?.Invoke(true);
            }
            else
            {
                SelectedSamplePath = Path.Combine(UniqueSamplesDirectory, ProjectGuid, guid + ".wav");
                Events.BaseSampleSelected?.Invoke(false);
            }

            NewSampleSelected?.Invoke(SelectedSamplePath);
        }

        public void DeleteUniqueSample()
        {
            //Directory.Delete(path, true);
            //InitializeUniqueSamples();

            FileInfo fInfo = new FileInfo(SelectedSamplePath);

            var dirName = fInfo.Directory.Name;

            if(dirName.Contains("BaseSamples"))
            {
                Debug.Log("Attempted to delete base sample");
                return;
            }

            Directory.Delete(fInfo.Directory.FullName, true);

            RefreshUnique();
        }   
    }
}
