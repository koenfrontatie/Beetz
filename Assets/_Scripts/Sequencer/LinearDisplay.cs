using UnityEngine;

public class LinearDisplay : Displayer
{
    //[SerializeField] private float stepDist = 1f;


    public override void PositionSteps()
    {
        //float width = stepDist * (base.sequencer.StepAmount - 1);
        float x = transform.localPosition.x;

        for (int i = 0; i < base.sequencer.StepAmount; i++)
        {
            //Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x - (width / 2f), 0f, transform.localPosition.z), Quaternion.identity, transform);
            Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x, 0f, transform.localPosition.z), Quaternion.identity, transform);
            base.steps.Add(step);
            x += Config.CellSize ;
        }

        base.UpdateMaterials();
    }
}
