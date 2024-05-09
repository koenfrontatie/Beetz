using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryController : MonoBehaviour
{
    [SerializeField] Transform _contentParent;
    [SerializeField] InfoTile _infoTilePrefab;
    
    [SerializeField] List<InfoTile> _infoTiles = new List<InfoTile>();

    public void RefreshInfoTiles()
    {
        var customSampleCount = AssetBuilder.Instance.CustomSamples.IDC.Count;

        if (_infoTiles.Count == customSampleCount) return;


        if (_infoTiles.Count > 0)
        {
            for (int i = 0; i < _infoTiles.Count; i++)
            {
                if(_infoTiles[i].gameObject != null) Destroy(_infoTiles[i].gameObject);
            }
        }


        for (int i = 0; i < customSampleCount; i++)
        {   
            var tile = Instantiate(_infoTilePrefab, _contentParent);

            _infoTiles.Add(tile);

            _infoTiles[i].AssignSampleData(AssetBuilder.Instance.CustomSamples.IDC[i]);
        }
        
    }
}
