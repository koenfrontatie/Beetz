using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;
    public static GameAssets Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>("GameAssets");
                GameObject instance = Instantiate(prefab);
                _instance = instance.GetComponent<GameAssets>();
            }
            return _instance;
        }
    }
    public Step Step;
    public Sequencer Sequencer;

}
