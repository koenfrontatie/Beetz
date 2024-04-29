
using UnityEngine;

public static class Config
{
    //public static readonly Color CustomColor1 = new Color(0.5f, 0.2f, 0.8f); // Example of defining a color directly

    public static float CellSize = .5f;
    public static Color BodyColor = Color.green;
    public static Color DisplayColor = Color.white;
    public static Color ActiveStep = new Color32(89, 117, 166, 1);
    public static Color PassiveStep = new Color32(91, 57, 0, 1);

    public static float CircularStepDistance = .5f;
    public static float CircularRowDistance = .5f;
    public static int ToolbarCount = 10;

    public static bool Log = true;
}
