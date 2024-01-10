using UnityEngine;

public class GridDisplay : Displayer
{
    [SerializeField] private float stepDist = .3f;
    public override void PositionSteps()
    {
        float width = stepDist * (base.sequencer.StepAmount - 1);
        float x = transform.localPosition.x;

        for (int i = 0; i < base.sequencer.StepAmount; i++)
        {
            Step step = Instantiate(Prefabs.Instance.Step, new Vector3(x - (width / 2f), 0f, transform.localPosition.z), Quaternion.identity, transform);
            base.steps.Add(step);
            x += stepDist;
        }

        base.UpdateMaterials();
    }
}
