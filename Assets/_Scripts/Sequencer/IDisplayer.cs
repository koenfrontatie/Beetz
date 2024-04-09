using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayer
{
    abstract void SpawnSteps();

    abstract void UpdateStepColors();

    abstract Step GetStepFromIndex(int index);
}
