using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

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
    }
    
    //public SequencerInfo CreateNewSequencerInfo()
    //{
    //    var info = new SequencerInfo();
    //    info.ID = Guid.NewGuid().ToString();
    //    return info;
    //}

    public SequencerInfo CreateNewSequencerInfo()
    {
        var info = new SequencerInfo();
        info.ID = Guid.NewGuid().ToString();
        info.Dimensions = new Vector2(16, 1);
        return info;
    }

    public PlaylistInfo CreateNewPlaylistInfo()
    {
        var info = new PlaylistInfo();
        info.ID = Guid.NewGuid().ToString();
        info.SequencerPositions = new List<PositionIDPair>();
        info.SequencerCorners = new List<V2Pair>();
        return info;
    }

    //public static SequencerInfo FindSequencerInfo()
    //{

    //}
}

[System.Serializable]
public struct SampleInfo
{
    public string ID;
    public string Name;
    public int Template;
}

[System.Serializable]
public struct SequencerInfo
{
    public string ID;
    public DisplayType Type;
    public Vector2 Dimensions;
    public List<Tuple<Vector2, string>> Samples;
}

[System.Serializable]
public struct PlaylistInfo
{
    public string ID;
    public List<PositionIDPair> SequencerPositions;
    public List<V2Pair> SequencerCorners;
}

// instantiation structs

[System.Serializable]
public struct PositionIDPair
{
    public Vector2 Position;
    public string ID;
}
[System.Serializable]
public struct V2Pair
{
    public Vector2 one;
    public Vector2 two;
}



//public PlanetInfo CreateNewInfo(PlanetType planetType = PlanetType.Rhythmic, int segments = 4, int beatsPerSegment = 4)
//{
//    PlanetInfo planetInfo = new PlanetInfo();
//    planetInfo.id = Guid.NewGuid().ToString();
//    planetInfo.segments = segments;
//    planetInfo.beatsPerSegment = beatsPerSegment;
//    planetInfo.planetType = planetType;
//    var rand = new Random();
//    int pick = rand.Next(0, 4);
//    planetInfo.planetColor = (PlanetColor)pick;
//    pick = rand.Next(0, 4);
//    planetInfo.ringColor = (RingColor)pick;
//    planetInfo.soundFontIndex = planetType == PlanetType.Melodic ? 0 : -1;
//    return planetInfo;
//}