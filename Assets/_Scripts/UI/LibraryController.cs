using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileManagement;

public class LibraryController : MonoBehaviour
{
    [SerializeField] Transform _contentParent;
    [SerializeField] InfoTile _infoTilePrefab;

    [SerializeField] List<InfoTile> _templateTiles = new List<InfoTile>();
    [SerializeField] List<InfoTile> _infoTiles = new List<InfoTile>();

    private Dictionary<string, InfoTile> _tileDictionary = new Dictionary<string, InfoTile>();

    private void OnEnable()
    {
        //Events.CustomDataLoaded += RefreshInfoTiles;
        FileManager.SampleDeleted += OnSampleDeleted;
        FileManager.SampleCreated += OnSampleCreated;
        DataStorage.ProjectDataSet += OnProjectDataSet;
        GameManager.StateChanged += OnStateChanged;
    }

    private void OnSampleDeleted(string obj)
    {
        if (_tileDictionary.TryGetValue(obj, out var tile))
        {
            _infoTiles.Remove(tile);
            _tileDictionary.Remove(obj);
            Destroy(tile.gameObject);
        }
    }
    void OnProjectDataSet(ProjectData data)
    {
        _tileDictionary.Clear();

        if(_infoTiles.Count > 0)
        {
            foreach(var tile in _infoTiles)
            {
                Destroy(tile.gameObject);
            }
            _infoTiles.Clear();
        }
    }
    private void OnSampleCreated(string guid)
    {
        if (!_tileDictionary.ContainsKey(guid))
        {
            var tile = Instantiate(_infoTilePrefab, _contentParent);
            tile.AssignSampleData(guid);
            _infoTiles.Add(tile);
            _tileDictionary.Add(guid, tile);
        }
    }   

    public void RefreshInfoTiles()
    {
        var customSampleCount = FileManager.Instance.UniqueSamplePathCollection.Count;

        //foreach(var tile in _templateTiles)
        //{
        //    tile.AssignSampleData(tile.GetGUID()); // quick hacky fix needs to be better
        //}
        //// Clear existing tiles
        //foreach (var tile in _infoTiles)
        //{
        //    Destroy(tile.transform.gameObject);
        //}

        //_infoTiles.Clear();

        // Create new tiles based on current samples
        for (int i = 0; i < customSampleCount; i++)
        {
            var sampleId = FileManager.Instance.GuidFromPath(FileManager.Instance.UniqueSamplePathCollection[i]);
            if (!_tileDictionary.ContainsKey(sampleId))
            {
                var tile = Instantiate(_infoTilePrefab, _contentParent);
                tile.AssignSampleData(sampleId);
                _infoTiles.Add(tile);
                _tileDictionary.Add(sampleId, tile);
            }
        }
    }

    public void ClearFromDictionary(string guid)
    {
        if (_tileDictionary.ContainsKey(guid))
        {
            Destroy(_tileDictionary[guid].gameObject);
            _tileDictionary.Remove(guid);
        }
    }

    private void OnDisable()
    {
        FileManager.SampleDeleted -= OnSampleDeleted;
        FileManager.SampleCreated -= OnSampleCreated;
        DataStorage.ProjectDataSet -= OnProjectDataSet;

        GameManager.StateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
       if(state == GameState.Library)
        {
            RefreshInfoTiles();
        }
    }
}
