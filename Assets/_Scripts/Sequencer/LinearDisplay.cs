using UnityEngine;

public class LinearDisplay : Displayer
{
    //[SerializeField] private float stepDist = 1f;


    public override void PositionSteps()
    {
        //float width = stepDist * (base.sequencer.StepAmount - 1);
        float x = transform.localPosition.x;

        for (int r = 0; r < base.Sequencer.RowAmount; r++)
        {
            for (int i = 0; i < base.Sequencer.StepAmount; i++)
            {
                //Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x - (width / 2f), 0f, transform.localPosition.z), Quaternion.identity, transform);
                Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x + Config.CellSize * i, 0f, transform.localPosition.z - r * Config.CellSize), Quaternion.identity, transform.GetChild(0));
                base.Steps.Add(step);
            }
        }

        //base.UpdateMaterials();
    }
}
