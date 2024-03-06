using Unity.VisualScripting;
using UnityEngine;

public class CircularDisplay : Displayer
{
    [SerializeField] private float radius = 1f;
    public override void PositionSteps()
    {
        //float width = stepDist * (base.sequencer.StepAmount - 1);
        //float x = transform.localPosition.x;

        float angleStep = (360 / (float)base.Sequencer.StepAmount);

        for (int i = 0; i < base.Sequencer.StepAmount; i++)
        {
            float angle = -angleStep * i;
            float radians = Mathf.Deg2Rad * -angle;
            float x = Mathf.Sin(radians) * radius;
            float z = Mathf.Cos(radians) * radius;
            Vector3 spawnPos = new Vector3(x, 0, z);
            Quaternion spawnRot = Quaternion.Euler(0f, -angle, 0f);
            Step step = Instantiate(Prefabs.Instance.Step, transform);
            step.transform.localPosition = spawnPos;
            step.transform.localRotation = spawnRot;
            base.Steps.Add(step);
        }

        base.UpdateMaterials();
    }
}
