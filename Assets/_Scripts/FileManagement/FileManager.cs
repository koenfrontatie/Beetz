using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Android;
using System.Collections.Generic;
using System;
using FileManagement;
using JetBrains.Annotations;
using UnityEditor;

//namespace FileManagement
//{
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

         public TextureCollection _customIcons;

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

            Events.BioMakeUnique += MakeBioUniqueFromGuid;


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
    
            Events.BioMakeUnique -= MakeBioUniqueFromGuid;
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
            //BaseSamplesPathCollection.Clear();

            //var di = new DirectoryInfo(BaseSamplesDirectory);
            //var files = di.GetFiles().OrderByDescending(x => x.Name).ToList();

            //for(int i = 0; i < files.Count; i++)
            //{
            //    BaseSamplesPathCollection.Add(files[i].FullName);
            //}

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
            
            var newSampleDirectory = Path.Combine(Utils.SampleSavepath, ProjectGuid, newName);
            
            var newFilePath = Path.Combine(newSampleDirectory, newName + ".wav");
            
            //string removewav = SelectedSamplePath.Substring(0, SelectedSamplePath.Length - 4);

            SampleData sampleJson = await AssetBuilder.Instance.GetSampleData(AssetBuilder.Instance.SelectedGuid);

            sampleJson.ID = newName;
        //var template;
            int template = sampleJson.Template;

            var ico = _customIcons.Collection[template];

            byte[] _bytes = ico.EncodeToPNG();
            await CheckOrMakeDirectory(newSampleDirectory);

            var jsonstring = Path.Combine(newSampleDirectory, newName + ".json");
            var pngstring = Path.Combine(newSampleDirectory, "ico.png");
            Debug.Log("from fmanager: " + pngstring);

        await Task.Run(() =>
            {
                if (!File.Exists(Path.Combine(newFilePath)))
                {
                    //var data = BetterStreamingAssets.ReadAllBytes(path);
                    var data = File.ReadAllBytes(path);

                    //var sampleJson = await AssetBuilder.Instance.GetSampleData(SelectedSamplePath);
                    SaveLoader.Instance.SaveData(jsonstring, sampleJson); // write json
                    // create new unique sample folder
                    //var newPath = Path.Combine(UniqueSamplesDirectory, Path.GetFileName(path));
                    // copy sample file into new folder
                    //File.WriteAllBytes(Path.Combine(newFilePath), data);
                    File.WriteAllBytes(pngstring, _bytes);
                    //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
                    File.WriteAllBytes(Path.Combine(newFilePath), data);
                }
            });

            SelectedSamplePath = newFilePath;
        }

        public async void MakeSelectedIntoUnique(string guid)
        {
            SetSelectedSamplePath(guid);

            await MakeSamplePathIntoUnique(SelectedSamplePath);

            RefreshUnique();

            //Events.BioMakeUniqueComplete?.Invoke();
        }

        public async void MakeBioUniqueFromGuid(string guid)
        {
            if (guid.Length < 3)
            {
                //SelectedSamplePath = BaseSamplesPathCollection[int.Parse(guid)];
                SelectedSamplePath = BaseSamplesDirectory + "/" + BaseSampleFromGuid(guid) + ".wav";
            }
            else
            {
                SelectedSamplePath = Path.Combine(UniqueSamplesDirectory, guid + ".wav");
            }

            await MakeSamplePathIntoUnique(SelectedSamplePath);

            Events.BioMakeUniqueComplete?.Invoke();

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
                //SelectedSamplePath = BaseSamplesPathCollection[int.Parse(guid)];
                SelectedSamplePath = BaseSamplesDirectory + "/" + BaseSampleFromGuid(guid) + ".wav";
                Events.BaseSampleSelected?.Invoke(true);
            }
            else
            {
                SelectedSamplePath = Path.Combine(UniqueSamplesDirectory, guid + ".wav");
                Events.BaseSampleSelected?.Invoke(false);
            }

            NewSampleSelected?.Invoke(SelectedSamplePath);
        }

        public string PathFromGuid(string guid)
        {
            if (guid.Length < 3)
            {
                return BaseSamplesDirectory + "/" + BaseSampleFromGuid(guid) + ".wav";
            }
            else
            {
                return Path.Combine(UniqueSamplesDirectory, guid + ".wav");
            }

            return null;
        }

        public void DeleteUniqueSample()
        {
            //Directory.Delete(path, true);
            //InitializeUniqueSamples();

            //FileInfo fInfo = new FileInfo(SelectedSamplePath);

            var dirName = Path.GetDirectoryName(SelectedSamplePath);

            if (dirName.Contains("BaseSamples"))
            {
                Debug.Log("Attempted to delete base sample");
                return;
            }
            Events.DeleteTile?.Invoke(AssetBuilder.Instance.SelectedGuid);
            //Debug.Log(fInfo.Directory.FullName);
            Directory.Delete(Path.Combine(dirName, AssetBuilder.Instance.SelectedGuid), true);

            Events.SetSelectedGuid?.Invoke("0");
            RefreshUnique();
        }
        private string BaseSampleFromGuid(string guid)
        {

            switch (int.Parse(guid))
            {
                case 0:
                    return "1kick";

                case 1:
                    return "2hat";

                case 2:
                    return "3clap";

                case 3:
                    return "4cow";

                case 4:
                    return "5snare";

                case 5:
                    return "6kick808";

                case 6:
                    return "7tab05";

                case 7:
                    return "8khat3";

                case 8:
                    return "9walk";

                case 9:
                    return "10chant";

            }

            return "basesample404";

        }
    }
//}
