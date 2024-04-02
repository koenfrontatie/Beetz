using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prefabs : MonoBehaviour
{
    public static Prefabs Instance;

    public Sequencer Sequencer;
    public Sequencer CircularSequencer;

    public Step Step;

    public Material White;
    public Material Blue;
    public Material DarkBlue;
    public Material Green;

    public List<SampleObject> BaseObjects = new List<SampleObject>();
    public List<Texture2D> BaseIcons = new List<Texture2D>();

    public Camera CanvasCamera;

    public float CellSize = .5f;
    public Color BodyColor = Color.green;
    public Color DisplayColor = Color.white;
    public Color ActiveStep = Color.green;
    public Color PassiveStep = new Color(91f / 255f, 57f / 255f, 0f, 1f);
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
}
