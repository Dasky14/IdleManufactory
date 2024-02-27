using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBuildingSelection : MonoBehaviour
{
    public GameObject buildingPrefab;

    public void SetBuilding()
    {
        if (buildingPrefab != null)
        {
            ControlManager.GetInstance().SetBuildingPrefab(buildingPrefab);
        }
        else
        {
            Debug.LogWarning("No building prefab set!", gameObject);
        }
    }
}
