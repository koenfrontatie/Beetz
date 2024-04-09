using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridControlMode : MonoBehaviour
{
    virtual public GridControlMode HandleInput()
    {

        return this;
    }
}
