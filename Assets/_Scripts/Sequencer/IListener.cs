using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListener
{
    abstract int GetStepPosition();

    abstract int GetBeatPosition();

    abstract int GetBarPosition();
}
