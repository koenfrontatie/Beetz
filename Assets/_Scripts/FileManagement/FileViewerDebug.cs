using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FileManagement { 
    public class FileViewerDebug : MonoBehaviour
    {
        [SerializeField] FileButton _fileButtonPrefab;

        [SerializeField] Transform _persistentBaseSamples;
        [SerializeField] Transform _persistentUniqueSamples;

        [SerializeField] private TMPro.TextMeshProUGUI _selectedText;

        private CanvasGroup _canvasGroup;
        private FileManager _fileManager;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _fileManager = GameObject.FindObjectOfType<FileManager>();
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        _canvasGroup.ToggleCanvasGroup();
        //    }
        //}
        private void OnEnable()
        {
            FileManager.BaseSamplesInitialized += OnBaseSamplesInitialized;
            FileManager.UniqueSamplesInitialized += OnUniqueSamplesInitialized;
            FileManager.NewSampleSelected += OnNewSampleSelected;
        }

        private void OnNewSampleSelected(string obj)
        {
            _selectedText.text = obj;
        }

        public void OnUniqueSamplesInitialized()
        {
            for (int i = 0; i < _persistentUniqueSamples.childCount; i++)
            {
                if (_persistentUniqueSamples.GetChild(i).TryGetComponent<FileButton>(out var fileButton))
                {
                    Destroy(fileButton.gameObject);
                }
            }

            for (int i = 0; i < _fileManager.UniqueSamplesPathCollection.Count; i++)
            {
                FileButton fileButton = Instantiate(_fileButtonPrefab, _persistentUniqueSamples);
                fileButton.AssignPathToButton(_fileManager.UniqueSamplesPathCollection[i]);
                var button = fileButton.GetComponent<Button>();
                button.onClick.AddListener(() => _fileManager.SetSelectedSamplePath(fileButton.Path));
            }
        }

        public void OnBaseSamplesInitialized()
        {
            for (int i = 0; i < _persistentBaseSamples.childCount; i++)
            {
                if (_persistentBaseSamples.GetChild(i).TryGetComponent<FileButton>(out var fileButton))
                {
                    Destroy(fileButton.gameObject);
                }
            }

            for (int i = 0; i < _fileManager.BaseSamplesPathCollection.Count; i++)
            {
                FileButton fileButton = Instantiate(_fileButtonPrefab, _persistentBaseSamples);
                fileButton.AssignPathToButton(_fileManager.BaseSamplesPathCollection[i]);
                var button = fileButton.GetComponent<Button>();
                button.onClick.AddListener(() => _fileManager.SetSelectedSamplePath(fileButton.Path));
            }
        }

        private void OnDisable()
        {
            FileManager.BaseSamplesInitialized -= OnBaseSamplesInitialized;
            FileManager.UniqueSamplesInitialized -= OnUniqueSamplesInitialized;
            FileManager.NewSampleSelected -= OnNewSampleSelected;
        }
    }
}