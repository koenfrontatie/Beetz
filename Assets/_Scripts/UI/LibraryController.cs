using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryController : MonoBehaviour
{
    [SerializeField] Transform _contentParent;
    [SerializeField] InfoTile _infoTilePrefab;

    [SerializeField] List<InfoTile> _infoTiles = new List<InfoTile>();
    private Dictionary<string, InfoTile> _tileDictionary = new Dictionary<string, InfoTile>();

    //private void OnEnable()
    //{
    //    Events.CustomDataLoaded += RefreshInfoTiles;
    //    Events.DeleteTile += OnDeleteTile;
    //}

    private void OnDeleteTile(string obj)
    {
        if (_tileDictionary.TryGetValue(obj, out var tile))
        {
            Destroy(tile.gameObject);
            _infoTiles.Remove(tile);
            _tileDictionary.Remove(obj);
        }
    }

    public void RefreshInfoTiles()
    {
        var customSampleCount = AssetBuilder.Instance.CustomSamples.IDC.Count;

        //// Clear existing tiles
        //foreach (var tile in _infoTiles)
        //{
        //    Destroy(tile.transform.gameObject);
        //}

        //_infoTiles.Clear();

        // Create new tiles based on current samples
        for (int i = 0; i < customSampleCount; i++)
        {
            var sampleId = AssetBuilder.Instance.CustomSamples.IDC[i];
            if (!_tileDictionary.ContainsKey(sampleId))
            {
                var tile = Instantiate(_infoTilePrefab, _contentParent);
                tile.AssignSampleData(AssetBuilder.Instance.CustomSamples.IDC[i]);
                _infoTiles.Add(tile);
                _tileDictionary.Add(sampleId, tile);
            }
        }
    }

    //private void OnDisable()
    //{
    //    Events.CustomDataLoaded -= RefreshInfoTiles;
    //    Events.DeleteTile -= OnDeleteTile;
    //}
}
