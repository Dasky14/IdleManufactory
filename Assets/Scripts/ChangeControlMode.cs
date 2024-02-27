using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeControlMode : MonoBehaviour
{
    [SerializeField]
    private ControlManager.ControlMode controlMode;

    public void ChangeMode()
    {
        ControlManager.GetInstance().SetControlMode(controlMode);
    }
}
