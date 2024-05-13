using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Android;
using System.Collections.Generic;
using System;
using System.Linq;
using static PlasticGui.GetProcessName;

namespace FileManagement
{
    public class FileManager : MonoBehaviour
    {
        public string ProjectGuid {get; private set;}
        // selection
        public string SelectedSampleGuid { get; private set; }
        public string SelectedSamplePath { get; private set; }
        public string ProjectDirectory { get; private set; }

        //paths
        public string BaseSamplesDirectory { get; private set; }
        public string UniqueSampleDirectory { get; private set; }

        //known path collection
        public List<string> BaseSamplePathCollection = new List<string>();
        public List<string> UniqueSamplePathCollection = new List<string>();

        //events
        public static Action BaseSamplesInitialized;
        public static Action UniqueSamplesInitialized;
        public static Action<string> SelectNewSample;
        public static Action<string> NewSampleSelected;

        public TextureCollection _customIcons;

        [SerializeField] private DataStorage _dataStorage;
        [SerializeField] private AssetBuilder _assetBuilder;
        
        [SerializeField] private KVDW.Logger _logger;

        public static FileManager Instance { get; private set; }

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

            BaseSamplesDirectory = Path.Combine(Application.persistentDataPath, "BaseSamples");
        }

        void OnEnable()
        {
            SelectNewSample += (guid) => SetPathFromGuid(guid);
   
            GameManager.StateChanged += OnNewGameState;
            Events.SetSelectedGuid += (guid) =>
            {
                SetPathFromGuid(guid);
            };

            DataStorage.ProjectDataSet += (data) => InitializeSamples();
        }

        void OnDisable()
        {
            SelectNewSample += (guid) => SetPathFromGuid(guid);

            GameManager.StateChanged -= OnNewGameState;

            Events.SetSelectedGuid -= (guid) =>
            {
                SetPathFromGuid(guid);
            };

            DataStorage.ProjectDataSet -= (data) => InitializeSamples();

        }

        private void OnNewGameState(GameState state)
        {
            if (state == GameState.Init)
            {
                OpenLastProject();
                //_dataStorage.SetProjectData(_dataStorage.ProjectData);
            }
        }
        #region SampleInitialization
        async void InitializeSamples()
        {
            CheckAndroidPermissions();

            BetterStreamingAssets.Initialize();

            // --------------------------------------------------- directory initialization
            
            await CheckOrMakeDirectory(BaseSamplesDirectory);

            UniqueSampleDirectory = Path.Combine(Application.persistentDataPath, "Projects", ProjectGuid, "UniqueSamples");

            await CheckOrMakeDirectory(UniqueSampleDirectory);
            
            await InitializeBaseSamples();

            await InitializeUniqueSamples();

        }

        async Task CheckOrMakeDirectory(string directory)
        {
            if (Directory.Exists(directory)) return;

            await Task.Run(() =>
            {
                Directory.CreateDirectory(directory);
            });
        }

        async Task InitializeBaseSamples()
        {
            // --------------------------------------------------- make sure all base samples in streaming assets are in persistent data path

            var betterfiles = BetterStreamingAssets.GetFiles("BaseSamples", "*.wav", SearchOption.AllDirectories);
            
            BaseSamplePathCollection?.Clear();

            for (int i = 0; i < betterfiles.Length; i++)
            {
                var pathToCheck = Path.Combine(Application.persistentDataPath, betterfiles[i]);
                
                BaseSamplePathCollection.Add(pathToCheck);
                
                await Task.Run(() =>
                {
                    if (!File.Exists(pathToCheck))
                    {
                        Log($"Didn't find {betterfiles[i]}, copying");
                        var data = BetterStreamingAssets.ReadAllBytes(betterfiles[i]);
                        Log($"Writing {betterfiles[i]}");
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
            UniqueSamplePathCollection.Clear();

            // --------------------------------------------------- get all unique sample folders

            await Task.Run(() =>
            {
                var info = new DirectoryInfo(UniqueSampleDirectory);

                var samplefolders = info.GetDirectories();

                for (int i = 0; i < samplefolders.Length; i++)
                {
                    var files = samplefolders[i].GetFiles();

                    for (int j = 0; j < files.Length; j++)
                    {
                        if(files[j].Name.EndsWith(".wav"))
                            UniqueSamplePathCollection.Add(files[j].FullName);
                    }
                }
            });

            UniqueSamplesInitialized?.Invoke();
        }
        #endregion
        
        #region Sample creation
        public async void MakeSelectedIntoUnique()
        {
            await MakeSamplePathIntoUnique(SelectedSamplePath);
        }
        async Task MakeSamplePathIntoUnique(string path)
        {            
            var newName = SaveLoader.Instance.NewGuid();
            
            var newSampleDirectory = Path.Combine(UniqueSampleDirectory, newName);

            var newSamplePath = Path.Combine(newSampleDirectory, newName + ".wav");

            SampleData sampleJson = await AssetBuilder.Instance.GetSampleData(SelectedSampleGuid);

            sampleJson.ID = newName;
        
            int template = sampleJson.Template;

            var ico = _customIcons.Collection[template]; // should be assetbuilder method

            byte[] _bytes = ico.EncodeToPNG();
            
            await CheckOrMakeDirectory(newSampleDirectory);

            var jsonstring = Path.Combine(newSampleDirectory, "SampleData.json");
            var pngstring = Path.Combine(newSampleDirectory, "ico.png");

            await Task.Run(() =>
            {
                if (!File.Exists(Path.Combine(newSamplePath)))
                {
                    var data = File.ReadAllBytes(path);

                    SaveLoader.Instance.SaveData(jsonstring, sampleJson); // write json
                    
                    File.WriteAllBytes(pngstring, _bytes);
                    
                    File.WriteAllBytes(Path.Combine(newSamplePath), data);
                }
            });

            SelectedSamplePath = newSamplePath;

            SelectedSampleGuid = newName;
        }
        public void DeleteUniqueSample()
        {
            var dirName = Path.GetDirectoryName(SelectedSamplePath);

            if (dirName.Contains("BaseSamples"))
            {
                Debug.Log("Attempted to delete base sample");
                return;
            }
            
            Events.DeleteTile?.Invoke(SelectedSampleGuid);
            
            Directory.Delete(Path.Combine(dirName, SelectedSampleGuid), true);

            Events.SetSelectedGuid?.Invoke("0");
            
            RefreshUnique();
        }
        public async void MakeGuidIntoUnique(string guid)
        {
            SetSelectedSamplePath(guid);

            await MakeSamplePathIntoUnique(SelectedSamplePath);

            RefreshUnique();
        }
        #endregion

        #region Utilities
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

        public void SetSelectedSamplePath(string path)
        {
            SelectedSamplePath = path;
            NewSampleSelected?.Invoke(SelectedSampleGuid);
        }

        public void SetPathFromGuid(string guid)
        {
            SelectedSampleGuid = guid;
            SelectedSamplePath = SamplePathFromGuid(guid);

            if(guid.Length < 3)
            {
                Events.BaseSampleSelected?.Invoke(true);
            }
            else
            {
                Events.BaseSampleSelected?.Invoke(false);
            }

            NewSampleSelected?.Invoke(guid);
        }

        public string GuidFromPath(string path)
        {
            var guid = Path.GetFileName(path);
            var removeWav = guid.Remove(guid.Length - 4, 4);
            Log($"Guid from path: {removeWav}");
            return removeWav;
        }
        public string SamplePathFromGuid(string guid)
        {
            if (guid.Length < 3)
            {
                return Path.Combine(BaseSamplesDirectory, BaseSampleNameFromGuid(guid) + ".wav");
            }
            else
            {
                return Path.Combine(UniqueSampleDirectory, guid, guid + ".wav");
            }
        }
        
        private string BaseSampleNameFromGuid(string guid)
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
        
        void Log(object message)
        {
            if (_logger)
                _logger.Log(message, this);
        }
        #endregion

        #region Project loading
        public async Task OpenNewProject()
        {
            var newGuid = SaveLoader.Instance.NewGuid();
            Log($"New project id is {newGuid}");
            
            //initialize folders
            ProjectGuid = newGuid;
            ProjectDirectory = Path.Combine(Application.persistentDataPath, "Projects", newGuid);
            UniqueSampleDirectory = Path.Combine(ProjectDirectory, "UniqueSamples");
            Utils.CheckForCreateDirectory(ProjectDirectory);
            Utils.CheckForCreateDirectory(UniqueSampleDirectory);
            
            //create data from template
            var projectData = await SaveLoader.Instance.DeserializeProjectData(Path.Combine(Application.streamingAssetsPath, "ProjectData.json"));
            projectData.ID = newGuid;
            SaveLoader.Instance.SaveData(Path.Combine(ProjectDirectory, "ProjectData.json"), projectData);

            _dataStorage.SetProjectData(projectData);
        }
        public async void OpenLastProject()
        {
            var projectsFolder = Path.Combine(Application.persistentDataPath, "Projects");
            
            await CheckOrMakeDirectory(projectsFolder);

            var projectDirectories = new DirectoryInfo(projectsFolder).GetDirectories().OrderByDescending(d => d.LastWriteTime).ToList();
            
            Log("Opening last project...");

            if (projectDirectories.Count > 0) // if there are projects available, open the most recent
            {
                ProjectDirectory = projectDirectories[0].FullName;
                ProjectGuid = Path.GetFileName(ProjectDirectory);
                var projectDataPath = Path.Combine(ProjectDirectory, "ProjectData.json");
                Log($"Attempting to load projectdata from: {projectDataPath}");
                var projectData = await SaveLoader.Instance.DeserializeProjectData(projectDataPath);
                //OnProjectDataLoaded(projectData);
                Log("Project data loaded.");

                _dataStorage.SetProjectData(projectData);

            }
            else // if there are no projects available, open a new project
            {
                await OpenNewProject();

                Log("No projects found, opening new.");
            }

            //Events.ProjectDataLoaded?.Invoke(ProjectData);
        }
        #endregion
    }
}

